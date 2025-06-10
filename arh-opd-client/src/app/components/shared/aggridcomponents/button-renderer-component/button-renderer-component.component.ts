import { Component, OnInit } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams, IAfterGuiAttachedParams } from 'ag-grid-community';

@Component({
  selector: 'app-button-renderer',
 /*  template: `
    <button type="button" (click)="onClick($event)">{{label}}</button>
    ` */
    template: `<span>{{params}}</span>`,
})

export class ButtonRendererComponent implements ICellRendererAngularComp {
public params : any
  //params;
  label: string | undefined;

  agInit(params: any): void {
    //debugger;
    this.params = params;
    //this.params = params;
    //this.label = this.params.label || null;
    this.params = this.params.context.componentParent.dateformatter(this.params,'dd-MM-yyyy')
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