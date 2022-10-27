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
    if(environment.production){
      window.location.href = environment.loginUrl;
    }else{
      this.loginService.setCurrentUserAuthorizations("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIzNWZiYTlmMi1mYjczLTRjNWQtYTIwOS1lZWYwOGJhNzRiNDYiLCJVc2VySWQiOiIxNjU3NzgxODc4OCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6WyJJc0xveWFsdHlDcmVhdG9yIiwiSXNMb3lhbHR5UmVhZGVyIl0sIm5iZiI6MTY2Njg3MjM5NywiZXhwIjoxNjY2OTA0Nzk3LCJpc3MiOiJUT0tFTl9JU1NVRVIiLCJhdWQiOiJUT0tFTl9BVURJRU5DRSJ9.E9f2XSW7h00RI0HiuUZ5LK4zPYKdQdbEVRe0R3Hm8Y4",
      [
        {
          "moduleId": 1,
          "authorizationList" :[1,2,4,3]
        },
        {
          "moduleId": 2,
          "authorizationList" :[1,2,4,3]
        },
        {
          "moduleId": 3,
          "authorizationList" :[3,3,1,2,4]
        },
        {
          "moduleId": 4,
          "authorizationList" :[3,3]
        },
        {
          "moduleId": 5,
          "authorizationList" :[1,2,3]
        }
      ]);
      this.router.navigate([this.loginService.setRoute()]);
    }
  }
}
