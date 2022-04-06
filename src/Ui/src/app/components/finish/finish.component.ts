import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-finish',
  templateUrl: './finish.component.html',
  styleUrls: ['./finish.component.scss']
})
export class FinishComponent implements OnInit {
  @Input('cardTitle') cardTitle: any;
  @Input('subTitle') subTitle: any;

  constructor() {
  }

  ngOnInit(): void {
  }
}
