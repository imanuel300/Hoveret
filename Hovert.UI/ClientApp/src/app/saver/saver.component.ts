import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DOCUMENT } from '@angular/common';
//import { TenderSection } from '../services/httpGeneralService.service';
import { TenderBookletSection } from '../model/TenderBookletSection';
import { TenderTemplatesBookletSection } from '../model/TenderTemplatesBookletSections';
import { Filename } from '../model/Filename';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpGeneralService } from '../services/httpGeneralService.service';
import { AlertService } from '../services/_alert';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-saver-component',
  templateUrl: './saver.component.html'
})
export class SaverComponent {
  constructor(private titleService: Title,private router: Router, alertService: AlertService, public httpGeneralService: HttpGeneralService, http: HttpClient, @Inject('BASE_URL') baseUrl: string, @Inject(DOCUMENT) private document, private route: ActivatedRoute) {
    var options = { autoClose: 0, keepAfterRouteChange: false };
    route.params.subscribe(params => {
      params["Id"] != undefined ? this.TemplateId = params["Id"] : this.TemplateId = null; console.log(params);
    });
    this.httpGeneralService.GetData("WordEditor/GetTemplate/?Id=", this.TemplateId, null, null).subscribe((data: any) => {
      console.log(data);
      this.Template = data;
      this.titleService.setTitle(this.Template.Title);
    }, error => {
      alertService.error('שגיאה בקבלת נתונים מהשרת', options);
    })
  }
  public Template: any = [];
  public TemplateId: string;
  //public forecasts: WeatherForecast[];
  //public sectionToDisplay: TenderBookletSection = new TenderBookletSection(0, 0, 0, "", "");
  public sectionToDisplay: TenderTemplatesBookletSection = new TenderTemplatesBookletSection(null);
  public LinkToFile: string;
  // public _http: HttpClient;
  // public _ret: string;
  // public _baseUrl: string;
  public DisplayExportButton: boolean = true;
  public DisplayData: boolean = false;
  // public TenderId: any ;
  // public TenderNumber: any;
  // public TenderYear: any;

  public _filename: Filename;
  // public port: string = "52253"; // 8111   53166
  // public host: string = "http://localhost";
  public headers: any;
  public headers1: HttpHeaders;



  ngOnInit() { }

  GetData() {
    //if (this.httpGeneralService.TenderId == null)
    if (this.httpGeneralService.TenderYear == undefined) this.httpGeneralService.TenderYear = 2019;
    if (this.httpGeneralService.TenderNumber == undefined) this.httpGeneralService.TenderNumber = 409;
    this.httpGeneralService.GetData("WordEditor/BindDataToTemplate/?TemplateId=" + this.TemplateId + "&TenderYear=" + this.httpGeneralService.TenderYear + "&TenderNumber=" + this.httpGeneralService.TenderNumber, null, null, null)  // "http://localhost:53166/odata/TenderBookletSections?$orderby=TenderSectionId" ("http://localhost:53166/odata/TenderBookletSections(800)")
      .subscribe(result => {
        this.Template.Value = result;
        this.DisplayExportButton = true;
        
      }, error => { console.log(error); });

  }

  DownloadWordFile(): void {
    if (this.httpGeneralService.TenderYear == undefined) this.httpGeneralService.TenderYear = 2019;
    if (this.httpGeneralService.TenderNumber == undefined) this.httpGeneralService.TenderNumber = 409;
    this.httpGeneralService.GetData("WordEditor/DownloadWordFile/?TemplateId=" + this.TemplateId + "&TenderYear=" + this.httpGeneralService.TenderYear + "&TenderNumber=" + this.httpGeneralService.TenderNumber, null, null, null).subscribe((data: any) => {
      this.LinkToFile = "http://" + data;
      window.open(this.LinkToFile);
    }, e => console.log(e))
  }

  Convert(input: TenderTemplatesBookletSection): TenderBookletSection {

    let ret = new TenderBookletSection();
    ret.Id = input.Id != null ? input.Id : 0;
    ret.SectionBody = input.SectionBody != null ? input.SectionBody : '';
    ret.SectionNumber = input.SectionNumber != null ? input.SectionNumber : '';
    ret.TenderId = input.TenderId != null ? input.TenderId : 0;
    ret.TenderSectionId = input.TenderSectionId != null ? input.TenderSectionId : null;
    return ret;
  }
  EditTemplate(TemplateId: string) {
    // if (this.generalService.isMenahel) this.generalService.EditMode= true;
    console.log(TemplateId);
    this.router.navigateByUrl("/Weditor/" + TemplateId);
  }



}

  // SaveData() {

  //   this._http.get<WeatherForecast[]>(this._baseUrl + 'api/Save/Get').subscribe(result => {
  //     this.forecasts = result;
  //   }, error => console.error(error));



  // }



  // SendData(): void {
  //   this._ret = "";
  //   this.sectionToDisplay.Id = parseInt(this.TenderNumber);
  //   // let requestOptions = new requestOptions({ headers: this.headers, withCredentials: true });
  //   let url = this._baseUrl + "api/Save/Book";
  //   let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
  //   //this._http.post(this._baseUrl + "api/Save", this.sectionToDisplay).subscribe(r => { this._ret = <string>r; console.log(r) }, e => console.error(e))
  //   this.httpGeneralService.PostData(url, JSON.stringify(this.sectionToDisplay), { headers: headers, withCredentials: true }).subscribe(r => { this._ret = <string>r; console.log(r) }, e => console.error(e))

  //   //   { withCredentials: true }
  // }




  // SendDataToController(): void {


  //   let obj = {
  //     "Id": 2475,
  //     "TenderSectionId": 0,
  //     "TenderId": 0,
  //     "SectionNumber": "4/50/2",
  //     "SectionBody": "XXXXX"
  //   };
  //   let url = this._baseUrl + "Tender" + `/` + "Create";
  //   this.httpGeneralService.PostData(url, JSON.stringify(obj), null).subscribe(r => { console.log(r) }, e => console.log(e))
  //   ////////////////////////////////

  //   // let url = this._baseUrl + "api/Save";
  //   //  this._http.post(url, this.sectionToDisplay, { headers: this.headers1 }).subscribe(r => { this._ret = <string>r; console.log(r) }, e => console.error(e))

  //   //   { withCredentials: true }
  // }


  // SendPostObjToController(): void {

  //   let url = this._baseUrl + "Tender" + `/` + "Create";
  //   let um = JSON.stringify({
  //     "Id": 2475,
  //     "TenderSectionId": 0,
  //     "TenderId": 0,
  //     "SectionNumber": "4/50/2",
  //     "SectionBody": this.sectionToDisplay.SectionBody
  //   });

  //   let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

  //   this.httpGeneralService.PostData(url, um, { headers: headers }).subscribe(r => { console.log("Done") }, e => console.log(e))


  // }



/*
const authReq = req.clone({
  setHeaders: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  },
  withCredentials: true
});
*/

    //let url = this.host + ":" + this.port + "/odata/TenderBookletSections";
    // let um = JSON.stringify({
    //   "Id": 2475,
    //   "TenderSectionId": 0,
    //   "TenderId": 0,
    //   "SectionNumber": "4/50/2",
    //   "SectionBody": "XXXXX"
    // });
    // this.sectionToDisplay.Id = parseInt(this.TenderNumber);
    //let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    // , {headers: this.headers1, withCredentials: true}
    // this.sectionToDisplay.Id = parseInt(this.tenderId);
    // let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    // this._http.post(url, this.Convert(this.sectionToDisplay), {
    //   headers: this.headers1, withCredentials: true }).subscribe(r => { alert("חוברת המכרז נשמר בהצלחה"); }, e => console.log(e))








  // SendPut(): void {

  //   let obj = { id: '1' };//  { "value": "TEST" };
  //   let sObj = JSON.stringify(obj);
  //   let url = this._baseUrl + "api/Save/1";
  //   //this._http.post(this._baseUrl + "api/Save", this.sectionToDisplay).subscribe(r => { this._ret = <string>r; console.log(r) }, e => console.error(e))
  //   this._http.put(url, obj, { headers: this.headers, withCredentials: true }).subscribe(r => { this._ret = <string>r; console.log(r) }, e => console.error(e))

  //   //   { withCredentials: true }
  // }













  // public OldGetFn() {
  //   this._http.get<WeatherForecast[]>(this._baseUrl + 'api/Save/Get').subscribe(result => {
  //     this.forecasts = result;
  //   }, error => console.error(error));
  // }

  // getPort(window): void {
  //   let sPort = window.location.port;
  //   let iPort = parseInt(sPort, 10) - 1;
  //   this.port = iPort.toString(10);

  // }

  // TestAutomaticOL(): void {
  //   let sText: string;

  //   sText = "X";
  //   // this.sectionToDisplay = new TenderSection(0, 0, 0, sText, 1);

  // }






// interface ITenderSection {
//   Id: number;
//   Section: string;
//   ParentSection: string;
//   Text: string;
//   ConditionId: boolean;
// }

// interface WeatherForecast {
//   dateFormatted: string;
//   temperatureC: number;
//   temperatureF: number;
//   summary: string;
// }
