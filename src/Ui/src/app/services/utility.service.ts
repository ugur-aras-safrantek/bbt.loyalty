import {Injectable} from '@angular/core';
import {Router} from "@angular/router";
import {AbstractControl, ValidationErrors, ValidatorFn} from "@angular/forms";

@Injectable({
  providedIn: 'root'
})

export class UtilityService {

  constructor(private router: Router) {
  }

  convertBase64ToFile = (base64String, fileName, mimeType) => {
    let bstr = atob(base64String);
    let n = bstr.length;
    let uint8Array = new Uint8Array(n);
    while (n--) {
      uint8Array[n] = bstr.charCodeAt(n);
    }
    return new File([uint8Array], fileName, {type: mimeType});
  }

  redirectTo(url: string) {
    this.router.navigateByUrl('/', {skipLocationChange: true}).then(() => this.router.navigate([url]));
  }

  EndDateGreaterThanStartDateValidator(formGroup: any): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      let startDate = formGroup.getRawValue().startDate?.singleDate?.date;
      let endDate = formGroup.getRawValue().endDate?.singleDate?.date;
      if (startDate && endDate) {
        startDate = new Date(`${startDate.month}-${startDate.day}-${startDate.year}`);
        endDate = new Date(`${endDate.month}-${endDate.day}-${endDate.year}`);
        return startDate > endDate ? {'endDateGreaterThanStartDateValidator': true} : null;
      }
      return null;
    }
  }

  fileTypeValidator(): ValidatorFn {
    return (): ValidationErrors | null => {
      return {'fileTypeValidator': true};
    }
  }

  tcknValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (control.value.length == 11) {
        let value = control.value.toString();

        let isEleven = /^[0-9]{11}$/.test(value);
        //11 hane olmalidir.

        let isFirstCharZero = value.charAt(0) != "0";
        //Ilk rakam 0 olmamalidir.

        let totalX1 = 0;
        let totalX2 = 0;
        for (let i = 0; i < 9; i += 2) {
          totalX1 += parseInt(value.charAt(i));
          //1-3-5-7-9 toplami alinir.
        }
        for (let i = 1; i < 9; i += 2) {
          totalX2 += parseInt(value.charAt(i));
          //2-4-6-8 toplami alinir.
        }
        let isRuleX = ((totalX1 * 7) - totalX2) % 10 == parseInt(value.charAt(9));
        //Tek siradaki rakamlarin toplaminin 7 katindan cift siradaki rakamlarin
        //toplami cikarilip 10'a bolundugunde 10. siradaki rakami vermelidir.

        let totalY = 0;
        for (let i = 0; i < 10; i++) {
          totalY += parseInt(value.charAt(i));
        }
        let isRuleY = totalY % 10 == parseInt(value.charAt(10));
        //Ilk 10 siradaki rakamlarin toplami 10'a bolundugunde 11. siradaki rakami vermelidir.
        return isEleven && isFirstCharZero && isRuleX && isRuleY
          ? null
          : {'tcknValidator': true};
      } else {
        return null;
      }
    }
  }

  vknValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (control.value.length == 10) {
        let vkn = control.value;
        let tmp;
        let sum = 0;
        let lastDigit = parseInt(vkn.charAt(9));
        for (let i = 0; i < 9; i++) {
          let digit = parseInt(vkn.charAt(i));
          tmp = (digit + 10 - (i + 1)) % 10;
          sum = tmp == 9
            ? sum + tmp
            : sum + ((tmp * (Math.pow(2, 10 - (i + 1)))) % 9);
        }
        return lastDigit == (10 - (sum % 10)) % 10
          ? null
          : {'vknValidator': true};
      } else {
        return null;
      }
    }
  }
}
