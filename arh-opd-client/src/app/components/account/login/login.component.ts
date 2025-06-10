import { HttpClient } from '@angular/common/http';
import { Component, OnInit,  } from '@angular/core';
import { AsyncValidatorFn,FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ICaptcha,IOTPPassword,IUser } from '../../shared/models/models';
import { SharedService } from '../../shared/shared.service';
import { environment } from '../../../../environments/environment';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {  
  loginForm: FormGroup;
  returnUrl: string;
  registerForm: FormGroup;
  userTypeList: any[];
  errors: string[];
  
  constructor(private accountService: AccountService, private router: Router,
    private fb: FormBuilder, private ss: SharedService,
    private http: HttpClient,
    private activatedRoute: ActivatedRoute, private toastr: ToastrService) {
    }

  loginform = true;
  recoverform = false;
  otpform = false;
  registerform = false;
  logincaptcha = false;
  resetcaptcha = false;
  registercaptcha = false;

  loginCaptchaImg = '';
  loginCaptchSession = '';
  loginSignature = '';

  registerCaptchaImg = '';
  registerCaptchSession = '';
  registerSignature = '';

  resetCaptchaImg = '';
  resetCaptchSession = '';
  resetSignature = '';
  resetSessionCode = '';

  newPasswordReq = false;

  baseurl = environment.apiUrl;

  showRegisterForm() {
    this.loginform = false;
    this.registerform = true;
    this.otpform = false;
    this.recoverform = false;
    this.logincaptcha = false;
    this.registercaptcha = false;

    // this.ss.GetRegisterCaptcha()
    // .subscribe((cc: ICaptcha) => {
  
    //   this.registerCaptchaImg = 'data:image/jpg;base64,' + cc.img;
    //   this.registerCaptchSession = cc.sessionCode;
    // },
    // error => {
    //   this.toastr.error('Error fetching Captcha');
    // });
  }

  showRecoverForm() {
   this.loginform = true;
    this.recoverform = true;
    this.resetcaptcha = true;
    this.otpform = false;
    this.registerform = false;
    this.logincaptcha = false;
    this.registercaptcha = false;

    // this.ss.GetResetCaptcha()
    // .subscribe((cc: ICaptcha) => {
  
    //   this.resetCaptchaImg = 'data:image/jpg;base64,' + cc.img;
    //   this.resetCaptchSession = cc.sessionCode;
    // },
    // error => {
    //   this.toastr.error('Error fetching Captcha');
    // });
  }

  showLoginForm() {
    this.loginform = true;
    this.recoverform = false;
    this.resetcaptcha = false;
    this.otpform = false;
    this.registerform = false;
    this.logincaptcha = false;
    this.registercaptcha = false;
  }

  showLoginCaptcha() {
    this.logincaptcha = true;
    // this.loginform = false;
    this.recoverform = false;
    this.resetcaptcha = false;
    this.otpform = false;
    this.registerform = false;
    this.registercaptcha = false;
  }

  showRegisterCaptcha() {
    this.registercaptcha = true;
    this.logincaptcha = false;
    this.loginform = false;
    this.registerform = true;
    this.recoverform = false;
    this.resetcaptcha = false;
    this.otpform = false;
  }

  showOtpForm() {
    //let ut = this.loginForm.get('userType').value;
    let un = this.loginForm.get('userName')?.value ?? '';

    // if (ut === null || ut === '' || ut === 'select') {
    //   this.toastr.error('Select User Type');
    //    return
    //  }
    if (un === null || un.trim() === '') {
      this.toastr.error('Specify Login ID');
      return
    }

    let resetForm = {
      loginId: un,
      sessionCode: this.resetCaptchSession,
      captchaCode: this.loginForm.get('resetCaptcha')?.value ?? ''
    }

    this.ss.GetPasswordResetOTP(resetForm)
    .subscribe((x: any) => {
      this.recoverform = false;
      this.resetcaptcha = false;
      this.otpform = true;
      this.resetSessionCode = x.sessionCode;
      this.toastr.success(x.message);
    },
    e => {
      this.toastr.error('');
    });
  }

  ngOnInit(): void {
      /* this.ss.GetLoginCaptcha()
    .subscribe((cc: ICaptcha) => {
  
      this.loginCaptchaImg = 'data:image/jpg;base64,' + cc.img;
      this.loginCaptchSession = cc.sessionCode;
    },
    error => {
      this.toastr.error('Error fetching Captcha');
    });  
*/
this.returnUrl = this.activatedRoute.snapshot.queryParams[this.returnUrl];
    this.createLoginForm();
    this.createRegisterForm();

  }
  

  createLoginForm() {
    this.loginForm = this.fb.group({
   //   Email:new FormControl('', [Validators.required,Validators.email]),
    Email:new FormControl('', [Validators.required]),
      Password: new FormControl('', Validators.required),
    //  otp: new FormControl(''),
      newpassword: new FormControl(''),
      confirmpassword: new FormControl(''),
  //    loginCaptcha: new FormControl(''),
   //   resetCaptcha: new FormControl('')
    });
  }

  checklogin() {   
    if (this.loginForm.valid) {
         this.accountService.login(this.loginForm.value).subscribe((sesdata: any) => {
           this.router.navigateByUrl(this.returnUrl);
           console.log('Return URL '+this.returnUrl);
       },      
       error => {        
        const errorMessage = error?.error?.message?.trim() || 'An unexpected error occurred.';
  console.log('Toastr Triggered: ', errorMessage); // Debug log
  this.toastr.error('Login Failed - ' + errorMessage);      
      //   if (error.error !== undefined && error.error !== null && error.status !== 0) {
        //   this.toastr.error('Login Failed - ' + error.error.message);
        this.loginForm.reset(); 
         }          
    );
    }
  }

  loginCaptchaSubmit() {
    let lc = this.loginForm.get('loginCaptcha')?.value ?? '';
    if (lc === null || lc.toString().trim() === '') {
      return ;
    }

    let sesdata = {
    //  userName: this.loginForm.get('userName').value,
    //  password: this.loginForm.get('password').value,
    Email: this.loginForm.get('Email')?.value ?? '',
    Password: this.loginForm.get('Password')?.value ?? '',
      sessionCode: this.loginCaptchSession,
      //signature: this.loginSignature,
      captchaCode: lc
    }

    // this.ss.ContinueLogin(sesdata)
    //   .subscribe(() => {
    //     this.router.navigateByUrl(this.returnUrl);
    // });
    this.accountService.continuelogin(sesdata)
    .subscribe((res: any) => {
      if (res.status === 'Success') {
        this.loginform = false;
        this.router.navigateByUrl(this.returnUrl);
      }
      else {
        //  this.loginform = false;
        this.logincaptcha = false;
        this.newPasswordReq = true;
      }
    },
    error => {
    //   this.ss.GetLoginCaptcha()
    // .subscribe((cc: ICaptcha) => {
  
    //   this.loginCaptchaImg = 'data:image/jpg;base64,' + cc.img;
    //   this.loginCaptchSession = cc.sessionCode;
    // },
    // error => {
    //   this.toastr.error('Error fetching Captcha');
    // });  

    if (error.error !== undefined && error.error !== null && error.status !== 0) {
        this.toastr.error('Login Failed - ' + error.error.message);
      }
      else {
        this.toastr.error('Login Failed - ' + error.message);
      }
    });

  }

  createRegisterForm() {
    this.registerForm = this.fb.group ({

      displayName: [null, [Validators.required]],
      loginid: [null, [Validators.required]],
      email: [null, 
          [Validators.required] 
         //  , Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')],
         //  [this.validateEmailNotTaken()]     //  this is async validator
      ],
      //regpassword: [null, Validators.required],
      //regconfirmpassword: [null, Validators.required],
      plantcode: [null, Validators.required],
      //  regOTP: [null, Validators.required],
      registerCaptcha: [null, Validators.required]
    });
  }

  checkRegister() {
    var lf = this.registerForm.value;
    var rf = {
      displayName: lf.displayName,
      loginid: lf.loginid,
      email: lf.email + '@bharatpetroleum.in',
      //regpassword: lf.regpassword,
      //regconfirmpassword: lf.regconfirmpassword,
      plantcode: lf.plantcode
    };

    if (!this.registerForm.valid) {
      this.toastr.error('Incomplete Form');
      return;
    }


  }

  onSubmitRegister() {
    var lf = this.registerForm.value;
    var rf = {
      displayName: lf.displayName,
      loginid: lf.loginid,
      email: lf.email + '@bharatpetroleum.in',
      //regpassword: lf.regpassword,
      //regconfirmpassword: lf.regconfirmpassword,
      plantcode: lf.plantcode,
      sessionCode: this.registerCaptchSession,
      captchaCode: lf.registerCaptcha
    };

    if (!this.registerForm.valid) {
      this.toastr.error('Incomplete Form');
      return;
    }

    this.accountService.register(rf).subscribe(response => {
        this.toastr.success('Registration Successful !!');
        //  this.router.navigateByUrl('/');
        this.showLoginForm();
    }, 
    error => {
      // this.ss.GetRegisterCaptcha()
      // .subscribe((cc: ICaptcha) => {
    
      //   this.registerCaptchaImg = 'data:image/jpg;base64,' + cc.img;
      //   this.registerCaptchSession = cc.sessionCode;
      // },
      // err => {
      //   this.toastr.error('Error fetching Captcha');
      // });

      this.toastr.error('Unable to Register');
    });
  }

  // validateEmailNotTaken(): AsyncValidatorFn {
  //   return control => {
  //     return timer(500).pipe(
  //       switchMap(() => {
  //         if (!control.value) {
  //           return of(null);
  //         }
  //         return this.accountService.checkEmailExists(control.value).pipe(
  //           map(res => {
  //             return res ? {emailExists: true} : null;
  //           })
  //         );
  //       })
  //     );
  //   };
  // }

  SubmitOTP() {
    //  let ut = this.loginForm.get('userType').value;
    let un = this.loginForm.get('userName')?.value ?? '';

    // if (ut === null || ut === '' || ut === 'select') {
    //   this.toastr.error('Select User Type');
    //   return
    // }
    if (un === null || un.trim() === '') {
      this.toastr.error('Specify Login ID');
      return;
    }

    let otppassword = new IOTPPassword();
    //  otppassword.userType = ut;
    otppassword.userName = un;
    otppassword.otp = this.loginForm.get('otp')?.value ?? '';
    otppassword.newPassword = this.loginForm.get('newpassword')?.value ?? '';
    otppassword.confirmPassword = this.loginForm.get('confirmpassword')?.value ?? '';
    otppassword.sessionCode = this.resetSessionCode;

    this.ss.GetPasswordCheckOTP(otppassword)
      .subscribe((x:any) => {
        this.loginForm.get('password')?.setValue(null);

        this.loginform = true;
        this.recoverform = false;
        this.otpform = false;
        //  this.loginForm.get('')
        this.toastr.success(x.message);
      },
        e => {
          this.toastr.error(e.error.errorMessage);
        });
  }

  SubmitNewPassword() {
    let un = this.loginForm.get('userName')?.value ?? '';

    let otppassword = {
      userName: un,
      newPassword: this.loginForm.get('newpassword')?.value ?? '',
      confirmPassword: this.loginForm.get('confirmpassword')?.value ?? '',
      sessionCode: this.loginCaptchSession
    };

    this.accountService.SetNewPassword(otppassword)
      .subscribe((x:any) => {
        this.newPasswordReq = false;
        this.loginForm.get('password')?.setValue(null);
        this.toastr.success(x.message);
        this.router.navigateByUrl(this.returnUrl);
      },
      e => {
        this.toastr.error(e.error.errorMessage);
      });
  }

  DownloadApk() {
    let fileName = 'ehsapp.apk';
    this.http.get(this.baseurl + 'account/downloadappapk', { responseType: 'blob' as 'blob' })
    .subscribe((fileData: any) => {
    const blob: any = new Blob([fileData], { type: 'application/vnd.android.package-archive' });
    let link = document.createElement('a');
    if (link.download !== undefined) {
        let url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
    this.toastr.success('Downloaded successfully');
    },
    error => {
        this.toastr.error('Error downloading APK');
    });
  }

}
