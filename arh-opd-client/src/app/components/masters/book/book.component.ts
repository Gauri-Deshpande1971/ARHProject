import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GridOptions,ICellRendererParams } from 'ag-grid-community';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../../environments/environment'
import { FormBuilder,FormControl, FormGroup, Validators } from '@angular/forms';
import { SharedService } from '../../shared/shared.service';
import { IBook } from '../../shared/models/models';
import { AccountService } from '../../account/account.service';
import { DropdownOption, DropdownService } from '../../shared/dropdown.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.css']
})
export class BookComponent {
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
  currentFormData: IBook ; 
  deptcode=0;
  //  master data - Drop Downs
  user:any;
  dataSource: any;
  options: DropdownOption[] = [];
  
  constructor(private toastr: ToastrService, private http: HttpClient,private dropdownService: DropdownService,
      private fb: FormBuilder, private ss: SharedService,private accountService: AccountService,
    private route: ActivatedRoute) { }
      
  ngOnInit(): void {    
         const currentUser = sessionStorage.getItem('currentUser');
    if (currentUser) {
    this.user = JSON.parse(currentUser); // Convert back to a JavaScript object  
    this.LoadData();
    if (!this.options || this.options.length === 0) {
      this.dropdownService.getCategoryOptions().subscribe({
      next: (options: DropdownOption[]) => {            
        this.options = options;
      }
    });
    }
  }
    
    this.entryForm = this.fb.group({
      bookId: [0],  // default value, change as needed
      Title: ['', [Validators.required, Validators.maxLength(255)]],
      author: [''],
      Description: ['', Validators.maxLength(2000)],
      ISBN: ['', Validators.maxLength(50)],
      CategoryId: [null, Validators.required],
      PublishedYear: [null, [Validators.required,
        Validators.pattern(/^\d{4}$/)]],
      CoverImagePath: ['', Validators.maxLength(500)],
      CreatedAt: [new Date(), Validators.required]
    }); 
    this.route.paramMap.subscribe(params => {
    const id = params.get('id');
    if (id) {
      this.getBookById(+id); // Call function to load the book
    }
  });                       
  }
  getBookById(bookId: number) {debugger;
  this.ss.GetBookById(bookId).subscribe({
    next: (book: any) => {
      this.ShowEntryForm(book);
    },
    error: (err) => {
      console.error('Failed to fetch book', err);
      this.toastr.error('Failed to load book details');
    }
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
      this.entryForm.get('CoverImagePath')?.setValue(staticPath);
  
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
      this.ss.GetBooksList()
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
      if (cd.field === 'bookId') {
          cd.cellRenderer = (params: ICellRendererParams) => {
          return this.renderFunc({params: params, fieldname: 'bookId'});
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
    this.currentFormData = new IBook();
       
      this.FF('Title')?.setValue ('');
      this.FF('author')?.setValue('');
      this.FF('Description')?.setValue('');
      this.FF('ISBN')?.setValue('');
      this.FF('CategoryId')?.setValue(0);
      this.FF('PublishedYear')?.setValue(0);
      this.FF('CoverImagePath')?.setValue('');

      this.entryForm.updateValueAndValidity();
      this.formmode='edit'        
  }
  SaveFormData() {debugger;
      let data = this.currentFormData;
      data.title = this.FF('Title')?.value;  
      data.author=this.FF('author')?.value;
      data.description=this.FF('Description')?.value;
      data.categoryId=this.FF('CategoryId')?.value;
      data.publishedYear=this.FF('PublishedYear')?.value;
      data.coverImagePath=this.FF('CoverImagePath')?.value;
      data.isbn=this.FF('ISBN')?.value;
      if (this.entryForm.invalid) {
       // console.log('Form Valid');
        this.toastr.error('Inputs not proper');
        return;
      }
      debugger;
      this.http.post(this.baseurl + 'Book/Save', data)
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
  ShowEntryForm(data: IBook) {    debugger;
      this.formmode = 'edit';
      this.currentFormData = data;  
      this.FF('bookid')?.setValue(data.bookId); 
      this.FF('Title')?.setValue(data.title);
      this.FF('author')?.setValue(data.author);
      this.FF('ISBN')?.setValue(data.isbn);
      this.FF('PublishedYear')?.setValue(data.publishedYear);    
      this.FF('CoverImagePath')?.setValue(data.coverImagePath);
      console.log('Options length'+this.options.length);
      if (!this.options || this.options.length === 0) {
          this.dropdownService.getCategoryOptions().subscribe({
          next: (options: DropdownOption[]) => {            
            this.options = options;
            if (data.categoryId && options.some(opt => opt.value === data.categoryId)) {
              this.FF('CategoryId')?.setValue(data.categoryId);
              console.log('Match '+data.categoryId);
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
        this.FF('CategoryId')?.setValue(data.categoryId);
      }
  }
  get coverImagePath(): string | null {
    return this.entryForm.get('CoverImagePath')?.value || null;
  }     
}
  

