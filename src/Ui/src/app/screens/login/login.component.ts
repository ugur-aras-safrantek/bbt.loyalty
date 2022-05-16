import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Subject, takeUntil} from 'rxjs';
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {LoginService} from "../../services/login.service";
import {UserAuthorizationsModel} from "../../models/login.model";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  submitted = false;
  userId = '';
  returnUrl: string = '';

  constructor(private route: ActivatedRoute,
              private router: Router,
              private toastrHandleService: ToastrHandleService,
              private loginService: LoginService) {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '';
  }

  ngOnInit(): void {
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

  login() {
    this.submitted = true;
    if (this.userId != '') {
      this.loginService.login({userId: this.userId})
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: res => {
            if (!res.hasError && res.data) {
              if (res.data.length > 0) {
                this.loginService.setCurrentUserAuthorizations(this.userId, res.data);
                this.router.navigate([this.setRoute()]);
              } else {
                this.toastrHandleService.warning("Kullanıcı bulunamadı");
              }
            } else
              this.toastrHandleService.error(res.errorMessage);
          },
          error: err => {
            if (err.error)
              this.toastrHandleService.error(err.error);
          }
        });
    }
  }

  private setRoute() {
    if (this.returnUrl == '' || this.returnUrl == '/campaign-definition/list') {
      let currentUserAuthorizations: UserAuthorizationsModel = this.loginService.getCurrentUserAuthorizations();

      if (currentUserAuthorizations.campaignDefinitionModuleAuthorizations.view) {
        this.returnUrl = '/campaign-definition';
      } else if (currentUserAuthorizations.campaignLimitsModuleAuthorizations.view) {
        this.returnUrl = '/campaign-limits';
      } else if (currentUserAuthorizations.targetDefinitionModuleAuthorizations.view) {
        this.returnUrl = '/target-definition';
      }
    }
    return this.returnUrl;
  }
}
