import { IDept,IDesg } from "./models";

export class ImportData {
  headings: [] = []; // Ensure it is initialized as an array
  dataSource: any[] = [];  // Ensure it is initialized as an array
    errorMessage?: string | undefined;
    errorInFields?: [];
    Success?:string | undefined;
    uploadCode?: string | undefined;
    //status: string | undefined;
    //datastatus: string | undefined;
    //excel_code: string | undefined;
  }
