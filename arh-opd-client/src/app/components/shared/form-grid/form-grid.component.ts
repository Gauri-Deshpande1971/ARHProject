import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { GridOptions } from 'ag-grid-community';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../../environments/environment';
import { SharedService } from '../../shared/shared.service';
import { ColDef } from 'ag-grid-community';
import { formatDate } from '@angular/common';
@Component({
  selector: 'app-form-grid',
  templateUrl: './form-grid.component.html',
  styleUrls: ['./form-grid.component.scss'],
 })
export class FormGridComponent implements OnInit {
@Input() isFullHeight: boolean = true;
@Input() rowData: [];
//  @Input() columnDefs: any[];
@Input() formName: string;
@Input() controllerName: string;
@Output() renderCell : EventEmitter<any> = new EventEmitter();
@Input() gridOptions: GridOptions;

  public frameworkComponents:any;
  public context:any;

  baseurl: string = environment.apiUrl;
  colDefs: ColDef[] = [];
  fieldFormats: any;
 // colDefs = [];
  constructor(private http: HttpClient, private totastr:ToastrService, private ss: SharedService) { }

  ngOnInit(): void {
    // console.log("FormName: " + this.formName);

    this.LoadColumns();

    this.gridOptions = <GridOptions>{
      columnDefs: this.colDefs,

      onGridReady: () => {
        //  this.gridOptions.api.sizeColumnsToFit();
      }
    };

    // console.log('Here - 1');
  }

  LoadColumns() {
    this.http.get(this.baseurl + this.controllerName + '/getgridcols?FormName=' + this.formName)
      .subscribe((ret:any) => {
        this.colDefs = [];

        if (ret !== null && ret.length > 0) {
          ret.forEach((element: { fieldType: string; fieldName: any;fieldFormat:any; fieldHeading: any; isSortable: any; isVisible: any; width: any; isResizable: any; isFilterable: any; }) => {
              if (element.fieldType === 'date' || element.fieldType === 'datetime') {
                this.colDefs.push({
                  field: element.fieldName,
                  headerName: element.fieldHeading,
                  filter: 'agDateColumnFilter', //  element.isFilterable,
                  filterParams: filterParams,
                  sortable: element.isSortable,
                  hide: !element.isVisible,
                  width: element.width,
                  //  position: element.position,
                  resizable: element.isResizable,                  
                 // fieldFormat: element.fieldFormat
                });                
              }
              else if (element.fieldType === 'number' || element.fieldType === 'decimal' || element.fieldType === 'int32' || element.fieldType === 'float') {
                this.colDefs.push({
                  field: element.fieldName,
                  headerName: element.fieldHeading,
                  filter: 'agNumberColumnFilter', //  element.isFilterable,
                  sortable: element.isSortable,
                  hide: !element.isVisible,
                  width: element.width,
                  //  position: element.position,
                  resizable: element.isResizable,
              //    fieldFormat: element.fieldFormat
                });
              }
              else {
                this.colDefs.push({
                  field: element.fieldName,
                  headerName: element.fieldHeading,
                  filter: element.isFilterable,
                  sortable: element.isSortable,
                  hide: !element.isVisible,
                  width: element.width,
                  //  position: element.position,
                  resizable: element.isResizable,
                //  fieldFormat: element.fieldFormat
                });
              }
            
          });

          this.gridOptions = <GridOptions>{
                columnDefs: this.colDefs,

                onGridReady: () => {
                    this.gridOptions.api?.sizeColumnsToFit();
                }
              };
          }  
          this.renderCell.emit(this.gridOptions);
      },
      error => {
        // console.log(error);
        
        this.totastr.error("Error in fetching: " + error.error);
      });

  }

}

var filterParams = {
  comparator: function (filterLocalDateAtMidnight:Date, cellValue:any):number {
    // console.log(filterLocalDateAtMidnight, cellValue);

    var dateAsString = cellValue;
    if (dateAsString == null) return -1;
    var dateParts = dateAsString.substr(0, 10).split('-');
    var cellDate = new Date(
      Number(dateParts[2]),
      Number(dateParts[1]) - 1,
      Number(dateParts[0])
    );

    // console.log(cellDate);

    if (filterLocalDateAtMidnight.getTime() === cellDate.getTime()) {
      return 0;
    }

    if (cellDate < filterLocalDateAtMidnight) {
      return -1;
    }

    if (cellDate > filterLocalDateAtMidnight) {
      return 1;
    }
    else
      return 1;
  },
  // browserDatePicker: true,
};