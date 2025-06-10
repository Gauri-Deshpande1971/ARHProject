import {Component, Output} from "@angular/core";
import {ICellRendererParams} from "ag-grid-community";
import {AgRendererComponent} from "ag-grid-angular";

@Component({
  selector: 'app-boolean-render',
  templateUrl: './boolean-render.component.html', 
  styleUrls: ['./boolean-render.component.css']
  // template: `<span>{{params}}</span>`,
})
export class BooleanRenderComponent implements AgRendererComponent {
  public params!: ICellRendererParams;
  objVal = false;

    agInit(params: ICellRendererParams): void {
        this.params = params;
        this.objVal = params.value;
    }

    public invokeParentMethod() {
      this.objVal = !this.objVal;

      this.params.context.componentParent.checkboxUpdated(this.params.data, this.params.colDef!.field, this.objVal);  //`Row: ${this.params.node.rowIndex}, Col: ${this.params.colDef.headerName}`)
        //// console.log(this.objVal);
    }

    refresh(): boolean {
        return false;
    }

}

/*

@Component({
    selector: 'child-cell',
    template: `<span><button style="height: 20px" (click)="invokeParentMethod()" class="btn btn-info">Invoke Parent</button></span>`,
    styles: [
        `.btn {
            line-height: 0.5
        }`
    ]
})
export class ChildMessageRenderer implements AgRendererComponent {
    public params: ICellRendererParams;

    agInit(params: ICellRendererParams): void {
        this.params = params;
    }

    public invokeParentMethod() {
        this.params.context.componentParent.methodFromParent(`Row: ${this.params.node.rowIndex}, Col: ${this.params.colDef.headerName}`)
    }

    refresh(): boolean {
        return false;
    }
}
*/
