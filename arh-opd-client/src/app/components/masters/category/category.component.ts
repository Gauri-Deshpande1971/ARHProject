import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GridOptions,ICellRendererParams } from 'ag-grid-community';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../../environments/environment'
import { FormBuilder,FormControl, FormGroup, Validators } from '@angular/forms';
import { SharedService } from '../../shared/shared.service';
import { ICategory } from '../../shared/models/models';
import { AccountService } from '../../account/account.service';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.css']
})
export class CategoryComponent {isfull = true;
  //gridOptions: GridOptions;
  baseurl = environment.apiUrl;
  bulkuploadfile = '';
  gridOptions: any = {};
  //rowData:  [];
  //rowData: any[] = [];
  rowData:any = [];
  colDefs = [];
  disabledatasave = false;
  verifysuccessfull = false; 
  uploadCode = '';
  formmode='list';
  entryForm: FormGroup;
  currentFormData: ICategory ; 
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
        categoryid:new FormControl( [null, [Validators.required,Validators.pattern('^[0-9]*$')]]),
          categoryname:new FormControl( [null,Validators.required])
              });                          
  }
   FF(fieldname: string ) {
      if (fieldname === null || fieldname === '') {
          return this.entryForm;
      }
      return this.entryForm.get(fieldname);
  }
  LoadData() {debugger;
      this.ss.GetCategoryList()
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
      if (cd.field === 'categoryid') {
          cd.cellRenderer = (params: ICellRendererParams) => {
          return this.renderFunc({params: params, fieldname: 'categoryid'});
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
    this.currentFormData = new ICategory();
       
      this.FF('categoryname')?.setValue ('');
      this.entryForm.updateValueAndValidity();
      this.formmode='edit'        
  }
  SaveFormData() {debugger;
      let data = this.currentFormData;
      data.categoryname = this.FF('categoryname')?.value;  
      if (this.entryForm.invalid) {
       // console.log('Form Valid');
        this.toastr.error('Inputs not proper');
        return;
      }
      debugger;
      this.http.post(this.baseurl + 'Category/Save', data)
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
  ShowEntryForm(data: ICategory) {    debugger;
      this.formmode = 'edit';
      this.currentFormData = data;  
      this.FF('categoryid')?.setValue(data.categoryid); 
      this.FF('categoryname')?.setValue(data.categoryname); 
  }     
  }
