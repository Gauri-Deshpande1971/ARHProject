import { Component, OnInit } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
@Component({
  selector: 'app-hours-minutes',
 /*  templateUrl: './hours-minutes.component.html',
  styleUrls: ['./hours-minutes.component.css'] */
  template: `<span>{{params}}</span>`,
})
export class HoursMinutesComponent implements ICellRendererAngularComp {
  public params : any
  label: string | undefined;
  agInit(params: any): void {
    //debugger;
    this.params = params;
    this.params = this.params.context.componentParent.GetHoursAndMinutes(this.params.value)
  }

  refresh(params?: any): boolean {
    return true;
  }

  onClick($event: any) {
    if (this.params.onClick instanceof Function) {
      // put anything into params u want pass into parents component
      const params = {
        event: $event,
        rowData: this.params.node.data
        // ...something
      }
      this.params.onClick(params);

    }
  }

}
