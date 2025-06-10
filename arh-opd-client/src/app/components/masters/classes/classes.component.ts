import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GridOptions, ICellRendererParams } from 'ag-grid-community';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../../environments/environment';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { SharedService } from '../../shared/shared.service';
import { IClasses } from '../../shared/models/models';
import { AccountService } from '../../account/account.service';
import { DropdownOption, DropdownService } from '../../shared/dropdown.service';
@Component({
  selector: 'app-classes',
  templateUrl: './classes.component.html',
  styleUrls: ['./classes.component.css'],
})
export class ClassesComponent {
  coverImageUrl: string | null = null;
  isfull = true;
  //gridOptions: GridOptions;
  baseurl = environment.apiUrl;
  //bulkuploadfile = '';
  gridOptions: any = {};
  rowData: any = [];
  colDefs = [];
  disabledatasave = false;
  verifysuccessfull = false;
  uploadCode = '';
  formmode = 'list';
  entryForm: FormGroup;
  currentFormData: IClasses;
  deptcode = 0;
  //  master data - Drop Downs
  user: any;
  dataSource: any;
  options: DropdownOption[] = [];

  constructor(
    private toastr: ToastrService,
    private http: HttpClient,
    private dropdownService: DropdownService,
    private fb: FormBuilder,
    private ss: SharedService,
    private accountService: AccountService
  ) {}

  ngOnInit(): void {
    const currentUser = sessionStorage.getItem('currentUser');
    if (currentUser) {
      this.user = JSON.parse(currentUser); // Convert back to a JavaScript object
      this.LoadData();
      debugger;
      if (!this.options || this.options.length === 0) {
        this.dropdownService.getStreamNames().subscribe({
          next: (options: DropdownOption[]) => {
            this.options = options;
          },
        });
      }
    }

    this.entryForm = this.fb.group({
      classId: [0], // default value, change as needed
      Name: ['', [Validators.required, Validators.maxLength(255)]],
      StreamId: [null, Validators.required],
    });
  }
  // onFileSelected(event: Event): void {
  //   const input = event.target as HTMLInputElement;
  //   if (input.files && input.files.length > 0) {
  //     const file = input.files[0];
  //     const fileName = file.name;

  //     // You assume the image is already present in /assets/img/
  //     const staticPath = `/assets/img/${fileName}`;

  //     // Save the static path to your form
  //     this.entryForm.get('CoverImagePath')?.setValue(staticPath);

  //     // For previewing it immediately
  //     this.coverImageUrl = staticPath;
  //   }
  // }

  FF(fieldname: string) {
    if (fieldname === null || fieldname === '') {
      return this.entryForm;
    }
    return this.entryForm.get(fieldname);
  }
  LoadData() {
    debugger;
    this.ss.GetClassList().subscribe(
      (res: any) => {
        debugger;
        this.rowData = res;
      },
      (error) => {
        console.log(error);
        alert('Error in Upload - ' + error.error);
      }
    );
  }
  gridLoaded($event: any) {
    this.LoadData();
    this.LoadSpecialColumns($event);
  }
  LoadSpecialColumns($event: any) {
    $event.columnDefs.forEach((cd: any) => {
      if (cd.field === 'classId') {
        cd.cellRenderer = (params: ICellRendererParams) => {
          return this.renderFunc({ params: params, fieldname: 'classId' });
        };
      }
    });
  }
  renderFunc($event: any) {
    console.log('render');
    const element = document.createElement('span');
    element.innerHTML = '';
    if ('Undefined' == $event.params.value) {
      return element;
    } else {
      let template =
        '<span style="color:#FF0000 !important;">' +
        $event.params.value +
        '</span>';
      element.innerHTML = template;
      element.addEventListener('click', () => {
        this.ShowEntryForm($event.params.data);
        console.log($event);
      });
    }
    return element;
  }
  NewFormData() {
    this.currentFormData = new IClasses();
    this.FF('Name')?.setValue('');
    // this.FF('author')?.setValue('');
    // this.FF('Description')?.setValue('');
    // this.FF('ISBN')?.setValue('');
    this.FF('StreamId')?.setValue(0);
    // this.FF('PublishedYear')?.setValue(0);
    // this.FF('CoverImagePath')?.setValue('');

    this.entryForm.updateValueAndValidity();
    this.formmode = 'edit';
  }
  ShowEntryForm(data: IClasses) {
    debugger;
    this.formmode = 'edit';
    this.currentFormData = data;
    this.FF('classId')?.setValue(data.classId);
    this.FF('Name')?.setValue(data.name);
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
  SaveFormData() {
    debugger;
    let data = this.currentFormData;
    data.name = this.FF('Name')?.value;
    //data.author = this.FF('author')?.value;
    //data.description = this.FF('Description')?.value;
    data.streamId = this.FF('StreamId')?.value;
    //data.publishedYear = this.FF('PublishedYear')?.value;
    //data.coverImagePath = this.FF('CoverImagePath')?.value;
    //data.isbn = this.FF('ISBN')?.value;
    if (this.entryForm.invalid) {
      // console.log('Form Valid');
      this.toastr.error('Inputs not proper');
      return;
    }
    debugger;
    this.http.post(this.baseurl + 'Classes/Save', data).subscribe(
      (response: any) => {
        console.log('API Response:', response);
        this.toastr.success('Saved Successfully');
      },
      (error) => {
        console.error('API Error:', error);
        if (error.status === 400) {
          console.error('Validation Errors:', error.error);
          this.toastr.error(
            'Validation Failed: ' + JSON.stringify(error.error)
          );
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
