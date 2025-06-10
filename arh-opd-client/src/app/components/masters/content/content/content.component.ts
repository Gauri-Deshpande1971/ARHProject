import { Component , ElementRef, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GridOptions,ICellRendererParams } from 'ag-grid-community';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../../environments/environment'
import { FormBuilder,FormControl, FormGroup, Validators } from '@angular/forms';
import { SharedService } from '../../shared/shared.service';
import { IContent } from '../../shared/models/models';
import { AccountService } from '../../account/account.service';
import { DropdownOption, DropdownService } from '../../shared/dropdown.service';
import { ReactiveFormsModule } from '@angular/forms';
//import { FormGroup, FormControl } from '@angular/forms';
//import { AfterViewInit } from '@angular/core';

@Component({
  selector: 'app-content',
  templateUrl: './content.component.html',
  styleUrls: ['./content.component.css']
})

export class ContentComponent implements OnInit {
  coverImageUrl: string | null = null;
  isfull = true;
  //gridOptions: GridOptions;
  baseurl = environment.apiUrl;
  bulkuploadfile = '';
  gridOptions: any = {};
  rowData: any = [];
  colDefs = [];
  disabledatasave = false;
  verifysuccessfull = false; 
  uploadCode = '';
  formmode='list';
  entryForm: FormGroup;
  currentFormData: IContent; 
  deptcode=0;
  //  master data - Drop Downs
  user:any;
  dataSource: any;
  options: DropdownOption[] = [];
  categorytypelist: DropdownOption[] = [];
  selected:true;
  
  constructor(private toastr: ToastrService, private http: HttpClient,private dropdownService: DropdownService,
    private fb: FormBuilder, private ss: SharedService,private accountService: AccountService) { }
  
  
    ngOnInit(): void {
    //throw new Error('Method not implemented.');
    const currentUser = sessionStorage.getItem('currentUser');
    if (currentUser) {
    this.user = JSON.parse(currentUser); // Convert back to a JavaScript object  
    this.LoadData();
    if (!this.options || this.options.length === 0) {
      this.dropdownService.getCategoryOptions().subscribe({
      next: (options: DropdownOption[]) => {            
        this.options = options;
      },
      error: (err) => console.error('Failed to load category options', err)
    });
    }
    if (!this.categorytypelist || this.categorytypelist.length === 0) {
      this.dropdownService.getContenttypenames().subscribe({
        next: (options_1: DropdownOption[]) => {            
          this.categorytypelist = options_1;
        },
        error: (err) => console.error('Failed to load category options', err)
      });
    }
  }
  console.log('options',this.options);
  console.log('options_1',this.categorytypelist);
    this.entryForm = this.fb.group({
      content_id: [0],  // default value, change as needed
      Title: ['', [Validators.required, Validators.maxLength(255)]],
      Author: [''],
      FilePath: [''],
      Description: ['', Validators.maxLength(2000)],
      CategoryId: [null, Validators.required],
      Content_type_id: [null, Validators.required],
      ThubnailPath: ['', Validators.maxLength(500)],
      //PublishDate: [new Date(), Validators.required]
      PublishDate: [new Date().toISOString().substring(0, 10), Validators.required]
      // ISBN: ['', Validators.maxLength(50)],
     
      // PublishedYear: [null, [Validators.required,
      //   Validators.pattern(/^\d{4}$/)]],
     
    });                  
  }

 
onFileSelected(event: Event): void {
  const input = event.target as HTMLInputElement;
  if (input.files && input.files.length > 0) {
    const file = input.files[0];
    const fileName = file.name;

    // You assume the image is already present in /assets/img/
    const staticPath = `/assets/img/${fileName}`;

    // Save the static path to your form
    this.entryForm.get('ThubnailPath')?.setValue(staticPath);

    // For previewing it immediately
    this.coverImageUrl = staticPath;
  }
}

 FF(fieldname: string ) {
    if (fieldname === null || fieldname === '') {
        return this.entryForm;
    }
    return this.entryForm.get(fieldname);
}
LoadData() {debugger;
    this.ss.GetContentsList()
    .subscribe((res: any) => {  debugger;      
    this.rowData = res;
    },
    error => {
    console.log(error);
    alert('Error in Upload - ' + error.error);
    });
}
gridLoaded($event:any) {
    this.LoadData();
    this.LoadSpecialColumns($event);
}
LoadSpecialColumns($event:any) {
    $event.columnDefs.forEach((cd: any) => {
    if (cd.field === 'content_id') {
        cd.cellRenderer = (params: ICellRendererParams) => {
        return this.renderFunc({params: params, fieldname: 'content_id'});
        }
    }
    });
};
renderFunc($event:any) {
    console.log('render');
    const element = document.createElement('span');
    element.innerHTML = '';
    if ('Undefined' == $event.params.value) {
    return element;
    } else {
    let template = '<span style="color:#FF0000 !important;">' + $event.params.value + '</span>';
    element.innerHTML = template;
    element.addEventListener('click', () => {
        this.ShowEntryForm($event.params.data);
        console.log($event);
    });
    }
    return element;
}
NewFormData()
{
  this.currentFormData = new IContent();
     
    this.FF('Title')?.setValue ('');
    this.FF('Author')?.setValue('');
    this.FF('Description')?.setValue('');
    this.FF('Content_type_id')?.setValue(0);
    this.FF('CategoryId')?.setValue(0);
    this.FF('PublishDate')?.setValue('');
    this.FF('FilePath')?.setValue('');
    this.FF('ThubnailPath')?.setValue('');

    this.entryForm.updateValueAndValidity();
    this.formmode='edit'        
}
SaveFormData() {debugger;
    let data = this.currentFormData;
    data.title = this.FF('Title')?.value;  
    data.author=this.FF('Author')?.value;
    data.description=this.FF('Description')?.value;
    data.categoryid=this.FF('CategoryId')?.value;
    data.content_type_id=this.FF('Content_type_id')?.value;
    data.publishDate=this.FF('PublishDate')?.value;
    data.filePath=this.FF('FilePath')?.value;
    data.thumbnailPath=this.FF('ThubnailPath')?.value;
    if (this.entryForm.invalid) {
     // console.log('Form Valid');
      this.toastr.error('Inputs not proper');
      return;
    }
    debugger;
    this.http.post(this.baseurl + 'Content/Save', data)
.subscribe(
  (response: any) => {
    console.log('API Response:', response);
    this.toastr.success('Saved Successfully');      
  },
  (error) => {
    console.error('API Error:', error);
    if (error.status === 400) {
      console.error('Validation Errors:', error.error);
      this.toastr.error('Validation Failed: ' + JSON.stringify(error.error));
    } else {
      this.toastr.error('An error occurred');
    }
  }
);
}
CancelSave() { 
    this.formmode = 'list';
}
formatDate(dateString: string): string {
  const date = new Date(dateString);
  const year = date.getFullYear();
  const month = ('0' + (date.getMonth() + 1)).slice(-2);
  const day = ('0' + date.getDate()).slice(-2);
  return `${year}-${month}-${day}`;
}

// formatDate(dateInput: string | Date): string {
//   const date = new Date(dateInput);
//   const year = date.getFullYear();
//   const month = ('0' + (date.getMonth() + 1)).slice(-2);
//   const day = ('0' + date.getDate()).slice(-2);
//   return `${year}-${month}-${day}`;
// }

ShowEntryForm(data: IContent) {    
  debugger;
    this.formmode = 'edit';
    this.currentFormData = data;  
    this.FF('contentId')?.setValue(data.content_id); 
    this.FF('Title')?.setValue(data.title);
    this.FF('Author')?.setValue(data.author);
    this.FF('Description')?.setValue(data.description);
    //this.FF('PublishDate')?.setValue(data.publishDate);
    this.FF('PublishDate')?.setValue(this.formatDate(data.publishDate));    
    this.FF('CategoryId')?.setValue(data.categoryid);
    this.FF('Content_type_id')?.setValue(data.content_type_id);
    this.FF('FilePath')?.setValue(data.filePath);
    this.FF('ThubnailPath')?.setValue(data.thumbnailPath);
    //this.FF('CategoryId')?.setValue(data.categoryId);
    console.log('Options length'+this.options.length);
    if (!this.options || this.options.length === 0) {
        this.dropdownService.getCategoryOptions().subscribe({
        next: (options: DropdownOption[]) => {            
          this.options = options;
          if (data.categoryid && options.some(opt => opt.value === data.categoryid)) {
            this.FF('CategoryId')?.setValue(data.categoryid);
            console.log('Match '+data.categoryid);
          } else {
            this.FF('CategoryId')?.setValue(null); // or maybe the first option, if needed
          }
        },
        error: (err) => {
          console.error('Failed to load dropdown options:', err);
        }
      });
    } else {
      // ✅ Set category immediately if options are already loaded
      this.FF('CategoryId')?.setValue(data.categoryid);
    }
    if (!this.categorytypelist || this.categorytypelist.length === 0) {
      this.dropdownService.getContenttypenames().subscribe({
      next: (options_1: DropdownOption[]) => {            
        this.categorytypelist = options_1;
        if (data.content_type_id && options_1.some(opt => opt.value === data.content_type_id)) {
          this.FF('Content_type_id')?.setValue(data.content_type_id);
          console.log('Match '+data.content_type_id);
        } else {
          this.FF('Content_type_id')?.setValue(null); // or maybe the first option, if needed
        }
      },
      error: (err) => {
        console.error('Failed to load content_type dropdown...:', err);
      }
    });
  } else {
    // ✅ Set category immediately if options are already loaded
    this.FF('Content_type_id')?.setValue(data.content_type_id);
  }
}
get ThubnailPath(): string | null {
  return this.entryForm.get('ThubnailPath')?.value || null;
}     

}
