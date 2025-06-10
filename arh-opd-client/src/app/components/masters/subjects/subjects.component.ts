import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ISubject } from '../../shared/models/models';
import { GridOptions,ICellRendererParams } from 'ag-grid-community';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../../environments/environment'
import { FormBuilder,FormControl, FormGroup, Validators } from '@angular/forms';
import { SharedService } from '../../shared/shared.service';
import { AccountService } from '../../account/account.service';
import { ChartRefreshServiceService } from '../../shared/chart-refresh-service.service';
import { DropdownOption, DropdownService } from '../../shared/dropdown.service';
@Component({
  selector: 'app-subjects',
  templateUrl: './subjects.component.html',
  styleUrls: ['./subjects.component.css']
})
export class SubjectsComponent {
  isfull = true;
  //gridOptions: GridOptions;
  baseurl = environment.apiUrl;
  bulkuploadfile = '';
  gridOptions: any = {};
  rowData:any = [];
  colDefs = [];
  disabledatasave = false;
  verifysuccessfull = false; 
  uploadCode = '';
  formmode='list';
  entryForm: FormGroup;
  currentFormData: ISubject ; 
  deptcode=0;
  //  master data - Drop Downs
  user:any;
  dataSource: any;
  options: DropdownOption[] = [];
  constructor(private toastr: ToastrService, private http: HttpClient,
    private chartRefreshService: ChartRefreshServiceService,private dropdownService: DropdownService,
      private fb: FormBuilder, private ss: SharedService,private accountService: AccountService) { }
      
  ngOnInit(): void {    
      this.baseurl = environment.apiUrl;
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
        StreamId:new FormControl( [null, [Validators.required,Validators.pattern('^[0-9]*$')]]),
          name:new FormControl( [null,Validators.required])
              });                          
  }
   FF(fieldname: string ) {
      if (fieldname === null || fieldname === '') {
          return this.entryForm;
      }
      return this.entryForm.get(fieldname);
  }
  LoadData() {debugger;
      this.ss.GetSubjectList()
      .subscribe((res: any) => {        
      this.rowData = res;
      console.table(this.rowData);
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
      if (cd.field === 'subjectId') {
          cd.cellRenderer = (params: ICellRendererParams) => {
          return this.renderFunc({params: params, fieldname: 'subjectId'});
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
    this.currentFormData = new ISubject();
       
      this.FF('name')?.setValue ('');
      this.FF('StreamId')?.setValue(0);
      this.entryForm.updateValueAndValidity();
      this.formmode='edit'        
  }
  ShowEntryForm(data: ISubject) {
    debugger;
    this.formmode = 'edit';
    this.currentFormData = data;
    this.FF('SubjectId')?.setValue(data.subjectId);
    this.FF('name')?.setValue(data.name);
    // this.FF('author')?.setValue(data.author);
    // this.FF('ISBN')?.setValue(data.isbn);
    // this.FF('PublishedYear')?.setValue(data.publishedYear);
    // this.FF('CoverImagePath')?.setValue(data.coverImagePath);
    console.log('Options length' + this.options.length);
    if (!this.options || this.options.length === 0) {
      this.dropdownService.getStreamNames().subscribe({
        next: (options: DropdownOption[]) => {
          this.options = options;
          if (
            data.streamId &&
            options.some((opt) => opt.value === data.streamId)
          ) {
            this.FF('StreamId')?.setValue(data.streamId);
            console.log('Match ' + data.streamId);
          } else {
            this.FF('StreamId')?.setValue(null); // or maybe the first option, if needed
          }
        },
        error: (err) => {
          console.error('Failed to load dropdown options:', err);
        },
      });
    } else {
      // ✅ Set category immediately if options are already loaded
      this.FF('StreamId')?.setValue(data.streamId);
    }
  } 
  SaveFormData() {debugger;
      let data = this.currentFormData;
      data.name = this.FF('name')?.value;
      data.streamId = this.FF('StreamId')?.value;  
      if (this.entryForm.invalid) {
       // console.log('Form Valid');
        this.toastr.error('Inputs not proper');
        return;
      }
      debugger;
      this.http.post(this.baseurl + 'Subject/Save', data)
  .subscribe(
    (response: any) => {
      console.log('API Response:', response);
      this.toastr.success('Saved Successfully');
      debugger;
      this.chartRefreshService.triggerRefresh();
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
      
}
