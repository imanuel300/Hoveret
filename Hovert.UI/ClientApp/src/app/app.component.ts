import { Component } from '@angular/core';
import { Router, NavigationStart, NavigationEnd } from '@angular/router';
import { SpinnerVisibilityService } from 'ng-http-loader';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(private route: Router, private spinner: SpinnerVisibilityService) { }
  // title = 'app';
  isLoading: boolean = false;
  ngOnInit() {
    this.route.events.subscribe(
      event => {
        if (event instanceof NavigationStart) {
          //console.log("navigation starts");
          this.spinner.show();
        }
        else if (event instanceof NavigationEnd) {
          //console.log("navigation ends");
          let that = this;
          setTimeout(function () {
            that.spinner.hide();
          }, 100)
        }
      },
      error => {
        this.spinner.hide();
        console.log(error);
      })
  }
}
