import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-auth-check',
  templateUrl: './auth-check.component.html',
  styleUrls: ['./auth-check.component.scss']
})

export class AuthCheckComponent implements OnInit {
  @Input('authorization') authorization: boolean = false;

  constructor() {
  }

  ngOnInit(): void {
  }

}
