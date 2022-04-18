import {Injectable} from '@angular/core';
import {CanDeactivate} from '@angular/router';
import {FormChange} from "../models/form-change";
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class FormChangeCheckGuard implements CanDeactivate<FormChange> {

  canDeactivate(component: FormChange): Observable<boolean> | boolean {
    if (component.formChangeState) {
      component.openFormChangeAlertModal();
      return component.formChangeSubject.asObservable();
    } else {
      return true;
    }
  }

}
