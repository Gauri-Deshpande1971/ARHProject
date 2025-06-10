import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GridOptions,ICellRendererParams } from 'ag-grid-community';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../../environments/environment';
import { FormBuilder,FormControl, FormGroup, Validators } from '@angular/forms';
import { SharedService } from '../../shared/shared.service';
import { IContentType } from '../../shared/models/models';
import { AccountService } from '../../account/account.service';

@Component({
  selector: 'app-contenttypes',
  templateUrl: './contenttypes.component.html',
  styleUrls: ['./contenttypes.component.css']
})
export class ContenttypesComponent {
isfull = true;
  //gridOptions: GridOptions;
  baseurl = environment.apiUrl;
  bulkuploadfile = '';
  gridOptions: any = {};
  rowData:  [];
  colDefs = [];
  disabledatasave = false;
  verifysuccessfull = false; 
  uploadCode = '';
  formmode='list';
  entryForm: FormGroup;
  currentFormData: IContentType ; 
  deptcode=0;
  //  master data - Drop Downs
  user:any;
  dataSource: any;
  constructor(private toastr: ToastrService, private http: HttpClient,
      private fb: FormBuilder, private ss: SharedService,private accountService: AccountService) { }
      
  ngOnInit(): void {    
      this.baseurl = environment.apiUrl;
      const currentUser = sessionStorage.getItem('currentUser');
    if (currentUser) {
    this.user = JSON.parse(currentUser); // Convert back to a JavaScript object   
    this.LoadData();  
    }
    
      this.entryForm = this.fb.group({
        content_type_id:new FormControl( [null, [Validators.required,Validators.pattern('^[0-9]*$')]]),
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
      this.ss.GetContentTypesList()
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
      if (cd.field === 'content_type_id') {
          cd.cellRenderer = (params: ICellRendererParams) => {
          return this.renderFunc({params: params, fieldname: 'content_type_id'});
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
    this.currentFormData = new IContentType();
       
      this.FF('name')?.setValue ('');
      this.entryForm.updateValueAndValidity();
      this.formmode='edit'        
  }
  SaveFormData() {debugger;
      let data = this.currentFormData;
      data.name = this.FF('name')?.value;  
      if (this.entryForm.invalid) {;
        this.toastr.error('Inputs not proper');
        return;
      }
      debugger;
      this.http.post(this.baseurl + 'Content_type/Save', data)
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
  ShowEntryForm(data: IContentType) {    debugger;
      this.formmode = 'edit';
      this.currentFormData = data;  
      this.FF('content_type_id')?.setValue(data.content_type_id); 
      this.FF('name')?.setValue(data.name); 
  }     
  }
