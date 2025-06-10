import { Component, OnInit } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
@Component({
  selector: 'app-cells-event',
  templateUrl: './cells-event.component.html',
  styleUrls: ['./cells-event.component.css']
})
export class CellsEventComponent implements ICellRendererAngularComp {
  params : any;
  agInit(params: any): void {

    this.params = params;
}

public invokeParentMethod() {
  debugger;
  if(this.params.data.IMSvisitId != undefined || this.params.data.IMSvisitId != null)
    {
      this.params.context.componentParent.EditIMSVisitEntryForm(this.params.data.IMSvisitId)
    }
}
  refresh(): boolean {
    return false;
}

}
