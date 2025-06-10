import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class ChartRefreshServiceService { private refreshChartSource = new Subject<void>();

  refreshChart$ = this.refreshChartSource.asObservable();

  triggerRefresh() {debugger;
    this.refreshChartSource.next();
  }
}
