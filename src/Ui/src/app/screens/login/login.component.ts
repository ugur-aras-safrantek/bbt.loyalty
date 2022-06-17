import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Subject, takeUntil} from 'rxjs';
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {LoginService} from "../../services/login.service";
import {environment} from "../../../environments/environment";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  code: string = '';
  state: string = '';

  constructor(private route: ActivatedRoute,
              private router: Router,
              private toastrHandleService: ToastrHandleService,
              private loginService: LoginService) {
    this.route.queryParams.subscribe(params => {
      this.code = params['code'];
      this.state = params['state'];
      if (this.code && this.state) {
        this.loginService.login({code: this.code, state: this.state})
          .pipe(takeUntil(this.destroy$))
          .subscribe({
            next: res => {
              if (!res.hasError && res.data) {
                this.loginService.setCurrentUserAuthorizations(res.data.accessToken, res.data.authorizationList);
                this.router.navigate([this.loginService.setRoute()]);
              } else
                this.toastrHandleService.error(res.errorMessage);
            },
            error: err => {
              if (err.error)
                this.toastrHandleService.error(err.error);
            }
          });
      } else if (this.loginService.getUserLoginInfo()) {
        this.router.navigate([this.loginService.setRoute()]);
      }
    });
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  login() {
    window.location.href = environment.loginUrl;
  }
}
