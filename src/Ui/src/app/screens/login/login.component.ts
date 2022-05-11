import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Subject, takeUntil} from 'rxjs';
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {LoginService} from "../../services/login.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  submitted = false;
  userId = '';

  constructor(private route: ActivatedRoute,
              private router: Router,
              private toastrHandleService: ToastrHandleService,
              private loginService: LoginService) {
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
                this.loginService.setCurrentUserAuthorizations(res.data);
                this.router.navigate(['']);
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
}
