import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Subject, takeUntil} from "rxjs";
import {LoginService} from "../../services/login.service";
import {ToastrHandleService} from 'src/app/services/toastr-handle.service';
import {GlobalVariable} from "../../global";
import {UserAuthorizationsModel} from "../../models/login.model";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})

export class HeaderComponent implements OnInit {
  private destroy$: Subject<boolean> = new Subject<boolean>();

  isCollapsed = false;

  currentUserAuthorizations: UserAuthorizationsModel = new UserAuthorizationsModel();

  constructor(private route: ActivatedRoute,
              private router: Router,
              private toastrHandleService: ToastrHandleService,
              private loginService: LoginService) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations();
  }

  ngOnInit(): void {
  }

  logout() {
    this.loginService.logout();
    this.router.navigate([GlobalVariable.login], {relativeTo: this.route});
  }

  clearCache() {
    this.loginService.clearCache()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: res => {
          if (res) {
            this.toastrHandleService.success();
          } else
            this.toastrHandleService.error("Bir sorun oluştu, lütfen daha sonra tekrar deneyin.");
        },
        error: err => {
          // if (err.error)
          //   this.toastrHandleService.error(err.error);
          this.toastrHandleService.error("Bir sorun oluştu, lütfen daha sonra tekrar deneyin. (500)");
        }
      });
  }
}
