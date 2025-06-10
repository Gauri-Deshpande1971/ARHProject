import { Component, EventEmitter, HostListener, Output } from '@angular/core';
import { INavMenu, IUser } from '../models/models';
import { Observable } from 'rxjs';
import { AccountService } from '../../account/account.service';
import { Router } from '@angular/router';
import { SharedService } from '../shared.service';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
declare var $: any;
@Component({
  selector: 'app-main-sidebar',
  templateUrl: './main-sidebar.component.html',
  styleUrls: ['./main-sidebar.component.css']
})
export class MainSidebarComponent { currentUser$: Observable<IUser | null>;
  user:any;
  menu_code:number[]=[];
  NavMenus: INavMenu[];
  sidemenustatus = false;
  showMenu = '0';
  showSubMenu = '';
  user_id:any;
 //institute_code=1;
  showClass: boolean = false;
  displayName: string = '';

  @Output() notify: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() actions: EventEmitter<any> = new EventEmitter<any>();

  handleNotify() {
    this.notify.emit(!this.showClass);
  }

  constructor(private accountService: AccountService, public router: Router,
      public ss: SharedService) { }

  ngOnInit(): void {
    this.currentUser$ = this.accountService.currentUser$;
       
    this.defaultSidebar = this.options.sidebartype;
    this.handleSidebar();
    const currentUser = sessionStorage.getItem('currentUser');
  if (currentUser) {
  debugger;
  this.user = JSON.parse(currentUser);
  this.user_id = this.user.user.user_id;

  this.ss.GetMenusByUserId(this.user_id)
    .subscribe((menuCodes: number[]) => {
      debugger;
      this.menu_code = menuCodes;
      console.log('Menu Codes:', this.menu_code);

      // Call second API **after** menu codes are available
      this.ss.GetUserNavmenus(this.menu_code)
        .subscribe((x: INavMenu[]) => {
          debugger;
          console.log(x);
          this.NavMenus = x;

          // Optional: delay expansion of UI if needed
          setTimeout(() => {
            this.addExpandClass('');
          }, 500);
        });
    });
} 
      $('[data-widget="treeview"]').Treeview("init");
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

  handleActions(actionName:any) {
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
