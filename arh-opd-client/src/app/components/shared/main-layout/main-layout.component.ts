import { Component } from '@angular/core';
import { AppComponent } from '../../../app.component'

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
 styleUrls: ['./main-layout.component.css'],
})
export class MainLayoutComponent {
  topbarMenuActive: boolean = false;
  overlayMenuActive: boolean = false;
  staticMenuDesktopInactive: boolean = false;
  staticMenuMobileActive: boolean = false;
  menuClick: boolean = false;
  topbarItemClick: boolean = false;
  activeTopbarItem: any;
  menuHoverActive: boolean = false;
  rightPanelActive: boolean = false;
  rightPanelClick: boolean = false;
  configActive: boolean = false;
  configClick: boolean = false;
  activePage: string = '';
   Userinfo: any = null;
   userModuleDetail: any[] = []; 
  user:any;
  username:any;
  constructor(
    
    public app: AppComponent,
   
  ) {}

   ngOnInit(): void {    
         const currentUser = sessionStorage.getItem('currentUser');
    if (currentUser) {
    this.user = JSON.parse(currentUser); 
    this.username=this.user.user.userName;
    }
}


  // onMenuButtonClick(event: any) {
  //   this.menuClick = true;
  //   this.topbarMenuActive = false;

  //   if (this.isOverlay()) {
  //     this.overlayMenuActive = !this.overlayMenuActive;
  //   }
  //   if (this.isDesktop()) {
  //     this.staticMenuDesktopInactive = !this.staticMenuDesktopInactive;
  //   } else {
  //     this.staticMenuMobileActive = !this.staticMenuMobileActive;
  //   }

  //   event.preventDefault();
  // }

  onMenuClick($event: any) {
    this.menuClick = true;
  }

  onTopbarMenuButtonClick(event: any) {
    this.topbarItemClick = true;
    this.topbarMenuActive = !this.topbarMenuActive;
    this.hideOverlayMenu();
    event.preventDefault();
  }

  onTopbarItemClick(event: any, item: any) {
    this.topbarItemClick = true;

    if (this.activeTopbarItem === item) {
      this.activeTopbarItem = null;
    } else {
      this.activeTopbarItem = item;
    }
    event.preventDefault();
  }

  onTopbarLogOutClick(event: any) {
    debugger;
    event.preventDefault();
    if (event.target.innerText === 'LogOut') {
      //this.authService.logout();
    }
  }

  onRightPanelButtonClick(event: any) {
    this.rightPanelClick = true;
    this.rightPanelActive = !this.rightPanelActive;
    event.preventDefault();
  }

  // onRippleChange(event: any) {
  //   this.app.ripple = event.checked;
  // }

  onConfigClick(event: any) {
    this.configClick = true;
  }

  onRightPanelClick() {
    this.rightPanelClick = true;
  }

  // isHorizontal() {
  //   return this.app.menuMode === 'horizontal';
  // }

  // isSlim() {
  //   return this.app.menuMode === 'slim';
  // }

  // isOverlay() {
  //   return this.app.menuMode === 'overlay';
  // }

  // isStatic() {
  //   return this.app.menuMode === 'static';
  //}

  isMobile() {
    return window.innerWidth < 1025;
  }

  isDesktop() {
    return window.innerWidth > 1024;
  }

  isTablet() {
    const width = window.innerWidth;
    return width <= 1024 && width > 640;
  }

  hideOverlayMenu() {
    this.overlayMenuActive = false;
    this.staticMenuMobileActive = false;
  }
}

