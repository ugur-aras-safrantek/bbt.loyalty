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
    this.loginService.logout();
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
    // window.location.href = environment.loginUrl;
    this.loginService.setCurrentUserAuthorizations('eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxOWY0OWE3Ni00NmQ4LTRiNDEtODdiMy0wMmEzOGM2NTVlNGMiLCJVc2VySWQiOiIyNzMwNzk3NzMxNiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6WyJJc0xveWFsdHlDcmVhdG9yIiwiSXNMb3lhbHR5QXBwcm92ZXIiLCJJc0xveWFsdHlSZWFkZXIiLCJJc0xveWFsdHlSdWxlQ3JlYXRvciIsIklzTG95YWx0eVJ1bGVBcHByb3ZlciJdLCJuYmYiOjE2NTkxMDE5MjUsImV4cCI6MTY1OTEzNDMyNSwiaXNzIjoiVE9LRU5fSVNTVUVSIiwiYXVkIjoiVE9LRU5fQVVESUVOQ0UifQ.V_yzSGr_4EmP2FI_lO9PVPyKZGdT3-qr_PnWv72z7Lg', [
      {
          "moduleId": 1,
          "authorizationList": [
              1,
              2,
              4,
              3
          ]
      },
      {
          "moduleId": 2,
          "authorizationList": [
              1,
              2,
              4,
              3
          ]
      },
      {
          "moduleId": 3,
          "authorizationList": [
              3,
              3,
              1,
              2,
              4
          ]
      },
      {
          "moduleId": 4,
          "authorizationList": [
              3,
              3
          ]
      },
      {
          "moduleId": 5,
          "authorizationList": [
              1,
              2,
              3
          ]
      }
  ]);
    this.router.navigate([this.loginService.setRoute()]);
  }
}
