import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Observable} from 'rxjs';
import {LoginService} from "../services/login.service";
import {ToastrHandleService} from "../services/toastr-handle.service";
import {UserAuthorizationsModel} from "../models/login.model";

@Injectable({
  providedIn: 'root'
})

export class AuthorityGuard implements CanActivate {

  constructor(private router: Router,
              private toastrHandleService: ToastrHandleService,
              private loginService: LoginService) {
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const currentUserAuthorizations = this.loginService.getCurrentUserAuthorizations();

    if (!this.hasAnyAuthority(currentUserAuthorizations)) {
      this.toastrHandleService.warning("Ekranları görüntüleme yetkiniz bulunmamaktadır.");
      this.loginService.logout();
      this.router.navigate(['/login']);
      return false;
    }

    let hasAuthority = false;
    switch (route.data.module) {
      case 'campaign-definition': {
        hasAuthority = currentUserAuthorizations.campaignDefinitionModuleAuthorizations.view;
        break;
      }
      case 'customer-definition': {
        hasAuthority = currentUserAuthorizations.customerDefinitionModuleAuthorizations.view;
        break;
      }
      case 'campaign-limits': {
        hasAuthority = currentUserAuthorizations.campaignLimitsModuleAuthorizations.view;
        break;
      }
      case 'target-definition': {
        hasAuthority = currentUserAuthorizations.targetDefinitionModuleAuthorizations.view;
        break;
      }
      case 'reports': {
        hasAuthority = currentUserAuthorizations.reportsModuleAuthorizations.view;
        break;
      }
    }

    if (!hasAuthority) {
      this.toastrHandleService.warning(`${route.data.moduleName} ekranını görüntüleme yetkiniz bulunmamaktadır.`);
    }

    return hasAuthority;
  }

  private hasAnyAuthority(currentUserAuthorizations: UserAuthorizationsModel) {
    if (currentUserAuthorizations.campaignDefinitionModuleAuthorizations.view ||
      currentUserAuthorizations.customerDefinitionModuleAuthorizations.view ||
      currentUserAuthorizations.campaignLimitsModuleAuthorizations.view ||
      currentUserAuthorizations.targetDefinitionModuleAuthorizations.view ||
      currentUserAuthorizations.reportsModuleAuthorizations.view) {
      return true;
    } else {
      return false;
    }
  }
}
