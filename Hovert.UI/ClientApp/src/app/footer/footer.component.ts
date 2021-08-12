import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html'
})
export class FooterComponent implements OnInit {
public appVersion : string = "1.2";
  constructor() { }

  ngOnInit(): void {
  }

}
