import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DOCUMENT } from '@angular/common';
//import { TenderSection } from '../services/httpGeneralService.service';
import { TenderBookletSection } from '../model/TenderBookletSection';
import { TenderTemplatesBookletSection } from '../model/TenderTemplatesBookletSections';
import { Filename } from '../model/Filename';
import { ActivatedRoute } from '@angular/router';
import { HttpGeneralService } from '../services/httpGeneralService.service';
 
@Component({
  selector: 'app-counter-component',
  templateUrl: './counter.component.html',
  styleUrls: ["./counter.component.css"]
})
export class CounterComponent {
  constructor(private activatedRoute: ActivatedRoute, private httpGeneralService: HttpGeneralService, http: HttpClient, @Inject('BASE_URL') baseUrl: string, @Inject(DOCUMENT) private document, private route: ActivatedRoute) {
    this._http = http;

    this._baseUrl = baseUrl;
    this.sectionToDisplay = new TenderTemplatesBookletSection(null);
    this._filename = new Filename(0, "");
    this.host = document.location.protocol + "//" + document.location.hostname;
    //if (window) {
    //  this.getPort(window);
    //}
    this.headers = new Headers();
    this.headers.append('Content-Type', 'application/json');
    this.headers.append('Accept', 'application/json');

    this.headers1 = new HttpHeaders();
    this.headers1.set("Content-Type", "application/json");//application/x-www-form-urlencoded
    this.headers1.set("Accept", "application/json");



  }


  //public forecasts: WeatherForecast[];
  //public sectionToDisplay: TenderBookletSection = new TenderBookletSection(0, 0, 0, "", "");
  public sectionToDisplay: TenderTemplatesBookletSection = new TenderTemplatesBookletSection(null);
  LinkToFile: string;
  _http: HttpClient;
  _ret: string;
  _baseUrl: string;
  cClasses: string = "centered";
  DisplayExportButton: boolean = false;
  DisplayData: boolean = false;
  tenderId: string = "2475";

  _filename: Filename;
  port: string = "52253"; // 8111   53166
  host: string = "http://localhost";
  headers: any;
  headers1: HttpHeaders;



  ngOnInit() {
    this.activatedRoute.queryParams.subscribe(params => { this.tenderId = params['tender']; });
    if (this.tenderId == null) this.tenderId = "2475";
  }

  GetData() {
    //this.sectionToDisplay;

    // this.httpGeneralService.GetData("TenderFilename(" + this.tenderId + ")", null, null, null)
    //   .subscribe(result => {
    //     this._filename = <Filename>result;
    //     this.sectionToDisplay.SectionNumber = this._filename.Name;
    //     console.log(result);
    //   }, error => console.log(error));

    //this.sectionToDisplay;

    this.httpGeneralService.GetData("TenderTemplatesBookletSections/GetTenderTemplatesBookletSection/?key=" + this.tenderId, null, null, null)  // "http://localhost:53166/odata/TenderBookletSections?$orderby=TenderSectionId" ("http://localhost:53166/odata/TenderBookletSections(800)")
      .subscribe(result => {
        let tempObject: TenderTemplatesBookletSection = <TenderTemplatesBookletSection>result[0];
        this.sectionToDisplay.SectionBody = tempObject.SectionBody;
        console.log(this.sectionToDisplay.SectionBody);
        if (this.tenderId != '0') this.DisplayExportButton = true;
        this.DisplayData = true;
      }, error => { console.log(error); });

  }


  // SaveData() {

  //   this._http.get<WeatherForecast[]>(this._baseUrl + 'api/Save/Get').subscribe(result => {
  //     this.forecasts = result;
  //   }, error => console.error(error));



  // }



  SendData(): void {
    this._ret = "";
    this.sectionToDisplay.Id = parseInt(this.tenderId);
    // let requestOptions = new requestOptions({ headers: this.headers, withCredentials: true });
    let url = this._baseUrl + "api/Save/Book";
    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    //this._http.post(this._baseUrl + "api/Save", this.sectionToDisplay).subscribe(r => { this._ret = <string>r; console.log(r) }, e => console.error(e))
    this.httpGeneralService.PostData(url, JSON.stringify(this.sectionToDisplay), { headers: headers, withCredentials: true }).subscribe(r => { this._ret = <string>r; console.log(r) }, e => console.error(e))

    //   { withCredentials: true }
  }




  SendDataToController(): void {


    let obj = {
      "Id": 2475,
      "TenderSectionId": 0,
      "TenderId": 0,
      "SectionNumber": "4/50/2",
      "SectionBody": "XXXXX"
    };
    let url = this._baseUrl + "Tender" + `/` + "Create";
    this.httpGeneralService.PostData(url, JSON.stringify(obj), null).subscribe(r => { console.log(r) }, e => console.log(e))
    ////////////////////////////////

    // let url = this._baseUrl + "api/Save";
    //  this._http.post(url, this.sectionToDisplay, { headers: this.headers1 }).subscribe(r => { this._ret = <string>r; console.log(r) }, e => console.error(e))

    //   { withCredentials: true }
  }


  SendPostObjToController(): void {

    let url = this._baseUrl + "Tender" + `/` + "Create";
    let um = JSON.stringify({
      "Id": 2475,
      "TenderSectionId": 0,
      "TenderId": 0,
      "SectionNumber": "4/50/2",
      "SectionBody": this.sectionToDisplay.SectionBody
    });

    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    this.httpGeneralService.PostData(url, um, { headers: headers }).subscribe(r => { console.log("Done") }, e => console.log(e))


  }

  SendPostObjToWEBAPI(): void {

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
    let um = JSON.stringify({
      "Id": 2475,
      "TenderSectionId": 0,
      "TenderId": 0,
      "SectionNumber": "4/50/2",
      "SectionBody": "XXXXX"
    });
    // this.sectionToDisplay.Id = parseInt(this.tenderId);
    //let headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    this.sectionToDisplay.Id = parseInt(this.tenderId);
    this.httpGeneralService.PostData("TenderBookletSections/Post", null, this.Convert(this.sectionToDisplay), null).subscribe((data: any) => {
      //alert("חוברת המכרז נשמרה בהצלחה <br>" + data.SectionBody);
      this.LinkToFile = "http://" + data.SectionBody;
    }, e => console.log(e))

    // , {headers: this.headers1, withCredentials: true}
    // this.sectionToDisplay.Id = parseInt(this.tenderId);
    // let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    // this._http.post(url, this.Convert(this.sectionToDisplay), {
    //   headers: this.headers1, withCredentials: true }).subscribe(r => { alert("חוברת המכרז נשמר בהצלחה"); }, e => console.log(e))


  }





  SendPut(): void {

    let obj = { id: '1' };//  { "value": "TEST" };
    let sObj = JSON.stringify(obj);
    let url = this._baseUrl + "api/Save/1";
    //this._http.post(this._baseUrl + "api/Save", this.sectionToDisplay).subscribe(r => { this._ret = <string>r; console.log(r) }, e => console.error(e))
    this._http.put(url, obj, { headers: this.headers, withCredentials: true }).subscribe(r => { this._ret = <string>r; console.log(r) }, e => console.error(e))

    //   { withCredentials: true }
  }













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


  Convert(input: TenderTemplatesBookletSection): TenderBookletSection {

    let ret = new TenderBookletSection();
    ret.Id = input.Id != null ? input.Id : 0;
    ret.SectionBody = input.SectionBody != null ? input.SectionBody : '';
    ret.SectionNumber = input.SectionNumber != null ? input.SectionNumber : '';
    ret.TenderId = input.TenderId != null ? input.TenderId : 0;
    ret.TenderSectionId = input.TenderSectionId != null ? input.TenderSectionId : null;
    return ret;
  }


}

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
