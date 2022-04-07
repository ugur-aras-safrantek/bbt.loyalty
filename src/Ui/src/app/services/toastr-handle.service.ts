import {Injectable} from '@angular/core';
import {ToastrService} from "ngx-toastr";

@Injectable({
  providedIn: 'root'
})

export class ToastrHandleService {

  constructor(private toastrService: ToastrService) {
  }

  success(message?: string) {
    this.toastrService.success(message);
  }

  error(error?: any) {
    if (error.hasError) {
      this.toastrService.error(error.errorMessage);
    } else if (error.title) {
      this.toastrService.error(error.title);
    } else {
      this.toastrService.error(error);
    }
  }
}
