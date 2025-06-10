import { Component, HostListener, OnInit, Output ,ViewChild, ElementRef  } from '@angular/core';
import { Chart, registerables } from 'chart.js';
Chart.register(...registerables);
import { RouterModule } from '@angular/router';
import { AccountService } from '../../account/account.service';
import { SharedService } from '../../shared/shared.service';
import { Router } from '@angular/router';
//import Chart from 'chart.js/auto';
import { Observable } from 'rxjs';
import { INavMenu,IStream,IUser, MostDownloadedContent } from '../../shared/models/models';
//import { Chart, ChartConfiguration, registerables } from 'chart.js';
//import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  //@ViewChild('mainChartCanvas') mainChartCanvas!: ElementRef;
  @ViewChild('mainChartCanvas', { static: true }) mainChartCanvas!: ElementRef;
  currentUser$: Observable<IUser | null> = new Observable<IUser | null>();
  purchaseOrderCount : number =0
  chartData: any[] = [];
  chart:any;
  user:any;
  activeusercount:number=0;
  totalcontent:number=0;
  memberregcount:number=0;
  NavMenus: INavMenu[] = [];
//   mostRead = {
//     id:1,
//   title: 'Atomic Habits',
//   author: 'James Clear',
//   count: 1348
// };
mostRead:MostDownloadedContent[]=[];
  currentMonthRange: string;
 selectedInterval = 'day';
  user_id: any;
  menu_code: number[]=[];
  constructor(private accountService: AccountService, public router: Router,
    public ss: SharedService) { }
  ngOnInit(): void {
    this.currentUser$ = this.accountService.currentUser$;
        const now = new Date();
        const monthName = now.toLocaleString('default', { month: 'long' });
        const year = now.getFullYear();
        
        // Example logic: showing "January - [Current Month] [Year]"
        this.currentMonthRange = `January - ${monthName} ${year}`;
    
    //this.defaultSidebar = this.options.sidebartype;
    //this.handleSidebar();
    const currentUser = sessionStorage.getItem('currentUser');

    if ( currentUser) {
      this.user = JSON.parse(currentUser);
       this.user_id=this.user.user.user_id;
      this.ss.GetMenusByUserId(this.user_id)
     .subscribe((menuCodes: number[]) => {
    this.menu_code = menuCodes; // this.menu_code should be a number[] type
    console.log('Menu Codes:', this.menu_code);
  }); 
      }
    this.ss.GetUserNavmenus(this.menu_code)
    .subscribe((x: INavMenu[]) => {
      console.log(x);
      this.NavMenus = x;

      setTimeout(() => {
        //this.addExpandClass('');
      }, 500);
    });
    this.ss.GetDownloadedContent()
    .subscribe((x: MostDownloadedContent[]) => {debugger;
      console.log(x);
      this.mostRead = x;
    });
    this.ss.GetActiveUsersCount()
    .subscribe((x: any) => {
      this.activeusercount = x;
    });
    this.ss.GetTotalcontent()
    .subscribe((x: any) => {
      this.totalcontent = x;
    });
    this.ss.GetCountMemberRegisterd()
    .subscribe((x: any) => {
      this.memberregcount = x;
    });
    this.GetTrafficData('day'); // initial load
  }
  onIntervalChange() {
    this.GetTrafficData(this.selectedInterval);
  }

 GetTrafficData(params: any): void {
  this.ss.GetTraffic(params).subscribe({
    next: (res: any) => {
      this.chartData = res;

      // Check if chartData is valid
      if (!this.chartData || !Array.isArray(this.chartData)) {
        console.error('Invalid chart data received');
        return;
      }

      const labels = this.chartData.map((d: any) => d.label);
      const values = this.chartData.map((d: any) => d.count);

      // Destroy previous chart instance to avoid overlapping
      if (this.chart) {
        this.chart.destroy();
      }

      const ctx = this.mainChartCanvas?.nativeElement?.getContext('2d');

      if (!ctx) {
        console.error('Canvas context not found');
        return;
      }

      this.chart = new Chart(ctx, {
        type: 'bar',
        data: {
          labels: labels,
          datasets: [{
            label: 'No. Of Registered Members',
            data: values,
            backgroundColor: 'rgba(60,141,188,0.9)',
            borderColor: 'rgba(60,141,188,1)',
            borderWidth: 1,
           // fill: true,
          //  tension: 0.3
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            y: {
              beginAtZero: true
            }
          }
        }
      });
    },
    error: (err) => {
      console.error('Error fetching traffic data:', err);
    }
  });
}
}