import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {LoginService} from "../../services/login.service";
import {GlobalVariable} from "../../global";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})

export class HeaderComponent implements OnInit {
  isCollapsed = false;

  constructor(private route: ActivatedRoute,
              private router: Router,
              private loginService: LoginService) {
  }

  ngOnInit(): void {
  }

  logout() {
    this.loginService.logout();
    this.router.navigate([GlobalVariable.login], {relativeTo: this.route});
  }
}
