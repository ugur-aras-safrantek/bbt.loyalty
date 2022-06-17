import {NgModule, LOCALE_ID} from '@angular/core';
import {LocationStrategy, PathLocationStrategy, registerLocaleData} from '@angular/common';
import localeTR from '@angular/common/locales/tr';
import {BrowserModule} from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {ToastrModule} from "ngx-toastr";
import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {LoginComponent} from "./screens/login/login.component";
import {DefaultLayoutComponent} from "./layouts/default-layout/default-layout.component";
import {HeaderComponent} from "./components/header/header.component";
import {HttpClientModule, HTTP_INTERCEPTORS} from "@angular/common/http";
import {FormsModule} from "@angular/forms";
import {SharedModule} from "./modules/shared.module";
import {HttpHeadersInterceptor} from "./helpers/http-headers.interceptor";
import {ErrorInterceptor} from "./helpers/error.interceptor";

registerLocaleData(localeTR);

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DefaultLayoutComponent,
    HeaderComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
      preventDuplicates: true,
    }),
    FormsModule,
    SharedModule,
  ],
  providers: [
    {provide: LOCALE_ID, useValue: 'tr-TR'},
    {provide: HTTP_INTERCEPTORS, useClass: HttpHeadersInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
  ],
  exports: [],
  bootstrap: [AppComponent]
})

export class AppModule {
}
