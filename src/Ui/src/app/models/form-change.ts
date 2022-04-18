import {Subject} from "rxjs";

export interface FormChange {
  formChangeSubject: Subject<boolean>;
  formChangeState: boolean;

  openFormChangeAlertModal: () => void;
}
