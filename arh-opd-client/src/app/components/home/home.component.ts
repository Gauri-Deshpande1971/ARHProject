import { Component, EventEmitter, HostListener, OnInit, Output } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountService } from '../account/account.service';
import { INavMenu,IStream,IUser } from '../shared/models/models';
import { Router } from '@angular/router';
declare var $: any;
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { SharedService } from '../shared/shared.service';
import Chart from 'chart.js/auto';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  currentUser$: Observable<IUser | null> = new Observable<IUser | null>();
  user:any;
  NavMenus: INavMenu[] = [];
  sidemenustatus = false;
  showMenu = '0';
  showSubMenu = '';
  menuType='';
  showClass: boolean = false;
  displayName: string = '';
  roleid:number=0;
  @Output() notify: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() actions: EventEmitter<any> = new EventEmitter<any>();
  chartData: any[] = [];
  chart:any;
  currentMonthRange: string;

  handleNotify() {
    this.notify.emit(!this.showClass);
  }

  constructor(private accountService: AccountService, public router: Router,
      public ss: SharedService) { }

  ngOnInit(): void {
        this.currentUser$ = this.accountService.currentUser$;
        const now = new Date();
        const monthName = now.toLocaleString('default', { month: 'long' });
        const year = now.getFullYear();
        
        // Example logic: showing "January - [Current Month] [Year]"
        this.currentMonthRange = `January - ${monthName} ${year}`;
    
    this.defaultSidebar = this.options.sidebartype;
    this.handleSidebar();
    const currentUser = sessionStorage.getItem('currentUser');

    if ( currentUser) {
      this.user = JSON.parse(currentUser);
      this.roleid=this.user.user.roleid;
      sessionStorage.setItem('roleid',this.roleid.toString());
      if(this.roleid==1) this.menuType='Admin';
      }
    this.ss.GetUserNavmenus(this.menuType)
    .subscribe((x: INavMenu[]) => {
      console.log(x);
      this.NavMenus = x;

      setTimeout(() => {
        this.addExpandClass('');
      }, 500);
    });
    this.GetTrafficData('day'); // initial load

    // setInterval(() => {
    //   this.GetTrafficData('day'); // refresh periodically
    // }, 10000); 
    $('[data-widget="treeview"]').Treeview("init");      
  }
  GetTrafficData(params:any)
  {
    this.ss.GetTraffic(params).subscribe(
      (res: any) => { 
        this.chartData = res;  
        if (this.chart) {
          this.chart.destroy();        
        }
        const labels = this.chartData.map((d: any) => d.label);   
          const values = this.chartData.map((d :any)=> d.count);
         this.chart=  new Chart('main-chart', {
            type: 'bar',
            data: {
              labels: labels,
              datasets: [{
                label: 'No. Of Registered Members',
                data: values,
                borderColor: 'blue',
               // fill: false
              }]
            }
          });// Call after data is ready
      });
  }
 
   ngAfterViewInit() {
    this.chart.redraw();
//     this.ss.GetStreamList()
//     .subscribe((data:any) => {
//   this.chartData = data;
  
//   const labels = this.chartData.map((d: any) => d.name);   
//     const values = this.chartData.map((d :any)=> d.streamId);
//     console.log('chart data'+labels[0]+" "+values);
//     new Chart('main-chart', {
//       type: 'line',
//       data: {
//         labels: labels,
//         datasets: [{
//           label: 'Stream Id as Metric',
//           data: values,
//           borderColor: 'blue',
//           fill: false
//         }]
//       }
//     });// Call after data is ready
// });
   //   const myChart = new Chart('main-chart', {
  //     type: 'line',
  //     data: {
  //       labels: ['Jan', 'Feb', 'Mar'],
  //       datasets: [{
  //         label: 'Traffic rate',
  //         data: [10, 20, 30],
  //         borderColor: 'blue'
  //       }]
  //     }
  //   });
   }
  
  sidemenuShowHide() {
    console.log(this.sidemenustatus);
    
    this.sidemenustatus = !this.sidemenustatus;
  }

  public config: PerfectScrollbarConfigInterface = {};
  active=1;

  tabStatus = 'justified';

  public isCollapsed = false;

  public innerWidth: any;
  public defaultSidebar: any;
  public showSettings = false;
  public showMobileMenu = false;
  public expandLogo = false;

  options = {
    theme: 'light', // two possible values: light, dark
    dir: 'ltr', // two possible values: ltr, rtl
    layout: 'vertical', // two possible values: vertical, horizontal
    sidebartype: 'full', // four possible values: full, iconbar, overlay, mini-sidebar
    sidebarpos: 'fixed', // two possible values: fixed, absolute
    headerpos: 'fixed', // two possible values: fixed, absolute
    boxed: 'full', // two possible values: full, boxed
    navbarbg: 'skin6', // six possible values: skin(1/2/3/4/5/6)
    sidebarbg: 'skin1', // six possible values: skin(1/2/3/4/5/6)
    logobg: 'skin1' // six possible values: skin(1/2/3/4/5/6)
  };

  Logo() {
    this.expandLogo = !this.expandLogo;
  }


  @HostListener('window:resize', ['$event'])
  onResize(event: string) {
    this.handleSidebar();
  }

  handleSidebar() {
    this.innerWidth = window.innerWidth;
    switch (this.defaultSidebar) {
      case 'full':
      case 'iconbar':
        if (this.innerWidth < 1170) {
          this.options.sidebartype = 'mini-sidebar';
        } else {
          this.options.sidebartype = this.defaultSidebar;
        }
        break;

      case 'overlay':
        if (this.innerWidth < 767) {
          this.options.sidebartype = 'mini-sidebar';
        } else {
          this.options.sidebartype = this.defaultSidebar;
        }
        break;

      default:
    }
  }
  // toggleMenu(menu: INavMenu): void {
  //   menu.isOpen = !menu.isOpen; // Toggle the isOpen property
  // }
  toggleMenu(menu: any): void {
    // Close all other sibling menus to avoid conflicts
    if (menu.parent_menu) {
      this.NavMenus.forEach((parent) => {
        if (parent.submenus) {
          parent.submenus.forEach((submenu) => {
            if (submenu !== menu) {
              submenu.isOpen = false;
            }
          });
        }
      });
    }
  
    // Toggle the clicked menu
    menu.isOpen = !menu.isOpen;
    console.log(`Toggled menu: ${menu.menu_description}, isOpen: ${menu.isOpen}`);
  }
  toggleSidebarType() {
    switch (this.options.sidebartype) {
      case 'full':
      case 'iconbar':
        this.options.sidebartype = 'mini-sidebar';
        break;

      case 'overlay':
        this.showMobileMenu = !this.showMobileMenu;
        break;

      case 'mini-sidebar':
        if (this.defaultSidebar === 'mini-sidebar') {
          this.options.sidebartype = 'full';
        } else {
          this.options.sidebartype = this.defaultSidebar;
        }
        break;

      default:
    }
  }

  handleClick(event: boolean) {
    this.showMobileMenu = event;
  }

  handleActions(actionName: string | null) {
    // console.log('handled - ' + actionName);

    if (actionName === 'logout') {
      this.logout();
      return;
    }

    if (actionName === 'myprofile') {
      this.router.navigate(['/account/myprofile']);
      return;
    }

    if (actionName === null || actionName === '')
      return;

    this.actions.emit(actionName);
  }

  logout() {
    this.accountService.logout();
  }

  addExpandClass(element: any) {
    if (element === this.showMenu) {
      this.showMenu = '0';
    } else {
      this.showMenu = element;
    }
  }

  addActiveClass(element: any) {
    if (element === this.showSubMenu) {
      this.showSubMenu = '0';
    } else {
      this.showSubMenu = element;
    }
    window.scroll({
      top: 0,
      left: 0,
      behavior: 'smooth'
    });
  }

}
