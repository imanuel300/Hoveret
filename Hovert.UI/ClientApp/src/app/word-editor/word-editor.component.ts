import { AfterViewInit, Component, ElementRef, HostListener, Inject, Injectable, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpGeneralService } from '../services/httpGeneralService.service';
import { FormControl, FormGroup, NgForm } from '@angular/forms';
import { EditorModule } from '@tinymce/tinymce-angular';
import { AlertService } from '../services/_alert'
import { ActivatedRoute, Router } from '@angular/router';
import { TenderTemplatesBookletSection } from '../model/TenderTemplatesBookletSections';
import { SpinnerVisibilityService } from 'ng-http-loader';
import { Alert, AlertType } from './../services/_alert/alert.model';
import { Subscription } from 'rxjs';
import { HashTable, KeyValuePair } from '../model/array.interface';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-word-editor',
  templateUrl: './word-editor.component.html'
})
//@Injectable()
export class WordEditorComponent {
  //@ViewChild('myEditor') myEditor: any;
  htmlEditor: any;
  public TemplateId: string = null;
  isParamsId: boolean = true;
  public loader: number = 225;
  public loaderRequestResult: any = [];
  public requestResult: any = [];
  public Lookup_MarketingMethodLookup: KeyValuePair<number, string>[] = [];
  public MarketingMethod: number = 21;
  public TenderNumber: number;
  public TenderYear: number;
  public bookmarks: any;
  public sumNewLine: number = 0;
  public Template: any = [];
  public EventOnChange: boolean = false;




  constructor(private titleService: Title, private router: Router, route: ActivatedRoute, alertService: AlertService, private elem: ElementRef, public httpGeneralService: HttpGeneralService, private http: HttpClient, @Inject('BASE_URL') baseUrl: string, private spinner: SpinnerVisibilityService) {
    // this.httpGeneralService.GetData('WordEditor/GetLookup', '/?table=MarketingMethodLookup', null, null).subscribe((data: any) => {
    //   this.Lookup_MarketingMethodLookup = data;
    // });


    var options = { autoClose: 0, keepAfterRouteChange: false };
    route.params.subscribe(params => {
      params["Id"] != undefined ? this.TemplateId = "/?Id=" + params["Id"] + "" : this.TemplateId = null; console.log(params);
    });
    this.httpGeneralService.GetData("WordEditor/GetTemplate", this.TemplateId, null, null).subscribe((data: any) => {
      console.log(data);
      this.Template = data;
      for (let i = 0; i < this.Template.length; i++) {
        this.Template[i].IsConditions = false;
      }

      //let title = this.titleService.getTitle();
      this.titleService.setTitle(this.Template.Title);


    }, error => {
      alertService.error('שגיאה בקבלת נתונים מהשרת', options);
    })
  }

  Getbookmarks() {
    if (this.TenderYear == undefined) this.TenderYear = 2019;
    if (this.TenderNumber == undefined) this.TenderNumber = 409;
    this.httpGeneralService.GetData('WordEditor/Getbookmarks', '/?TenderYear=' + this.TenderYear + '&TenderNumber=' + this.TenderNumber, null, null).subscribe((data: any) => {
      //console.log(data);
      this.bookmarks = data;
    });

  }
  onChange() {
    console.log("onChange");
    this.EventOnChange = true;
  }

  SaveToFile(TemplateId: string) {
    // if (this.generalService.isMenahel) this.generalService.EditMode= true;
    this.router.navigateByUrl("/Saver/" + TemplateId);
  }
  onSubmitSave(Template: any) {
    this.spinner.show();
    console.log(Template);
    this.httpGeneralService.PostData("WordEditor/SaveTemplate", null, Template, null).subscribe((data: any) => {
      console.log("data: ");
      console.log(data);
      this.Template = data;
      this.spinner.hide();
      this.EventOnChange = false;
    })
  }

  public editorForm: FormGroup;
  public Interval: any;
  ngOnInit() {

  }


  selectItem(item: any) {
    console.log(item);
    item.srcElement.style.height = 'auto';
    item.srcElement.style.height = item.srcElement.scrollHeight + 'px';
  }





}


