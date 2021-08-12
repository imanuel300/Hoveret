import { Component } from '@angular/core';
import { HttpGeneralService } from '../services/httpGeneralService.service';

@Component({
  selector: 'app-guide',
  templateUrl: './guide.component.html',
})
export class GuideComponent {
  public Templates: any;

  constructor(public httpGeneralService: HttpGeneralService) {
    
  }



}
