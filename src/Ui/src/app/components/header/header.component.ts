import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {LoginService} from "../../services/login.service";
import {GlobalVariable} from "../../global";
import {UserAuthorizationsModel} from "../../models/login.model";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})

export class HeaderComponent implements OnInit {
  isCollapsed = false;

  currentUserAuthorizations: UserAuthorizationsModel = new UserAuthorizationsModel();

  constructor(private route: ActivatedRoute,
              private router: Router,
              private loginService: LoginService) {
    this.currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations();
  }

  ngOnInit(): void {
  }

  logout() {
    this.loginService.logout();
    this.router.navigate([GlobalVariable.login], {relativeTo: this.route});
  }
}
