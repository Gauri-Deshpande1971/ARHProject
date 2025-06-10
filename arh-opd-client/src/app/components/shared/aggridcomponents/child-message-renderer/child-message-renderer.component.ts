import {Component} from "@angular/core";
import { ICellRendererAngularComp } from 'ag-grid-angular';

@Component({
    selector: 'child-cell',
    template: `<span style="color:#FF0000 !important" (click)="invokeParentMethod()">{{params.value}}</span>`,
    styles: [
        `.btn {
            line-height: 0.5
        }`
    ]
})
export class ChildMessageRenderer implements ICellRendererAngularComp {
    public params: any;

    agInit(params: any): void {

        this.params = params;
    }

    public invokeParentMethod() {
        //debugger;
        // console.log(this.params.context.componentParent.formname);
        if(this.params.context.componentParent.formname == "InstallBase")
        {
          this.params.context.componentParent.EditInstallBaseEntryForm(this.params.data.InstallBaseId)
        }
        if(this.params.context.componentParent.formname == "Ims")
        {
          this.params.context.componentParent.EditIMSEntryForm(this.params.data.IMSid)
        }

        if(this.params.context.componentParent.formname == "Contract")
        {
          if(this.params.data.ContractId != undefined || this.params.data.ContractId != null)
          {
            this.params.context.componentParent.EditContractEntryForm(this.params.data.ContractId)
          }
          
        }
          
      }

    refresh(): boolean {
        return false;
    }
}

