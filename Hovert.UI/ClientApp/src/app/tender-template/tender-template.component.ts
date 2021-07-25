import { Component, OnInit } from '@angular/core';
import { Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TenderTemplatesBookletSection } from '../model/TenderTemplatesBookletSections';
import { HttpGeneralService } from '../services/httpGeneralService.service';
import { TenderBookletSection } from '../model/TenderBookletSection';


@Component({
  selector: 'app-tender-template',
  templateUrl: './tender-template.component.html',
  styleUrls: ['./tender-template.component.css']
})
export class TenderTemplateComponent implements OnInit {
    ngOnInit(): void {
        
    }

   

  constructor(http: HttpClient, private httpGeneralService: HttpGeneralService, @Inject('BASE_URL') baseUrl: string) {
    this._http = http;
    this._baseUrl = baseUrl;
    this.sectionToDisplay = new TenderTemplatesBookletSection(null);
  }
  IsHtml : boolean = true;

  //public forecasts: WeatherForecast[];
  //public sectionToDisplay: TenderBookletSection = new TenderBookletSection(0, 0, 0, "", "");
  public sectionToDisplay: TenderTemplatesBookletSection = new TenderTemplatesBookletSection(null);
   _http: HttpClient;
   _ret: string;
   _baseUrl: string;
   cClasses: string = "centered";
   DisplayExportButton: boolean = false;
   tenderId: string = "0";
   _filename: string = "FileName";
  port: string = "52253"; // 8111   53166  

  // public GetDataForPreview() {  //  OldGetFn();
  //   this.TestAutomaticOL();
  //   //////////// this.GetData();
  // }

  Convert(input: TenderTemplatesBookletSection): TenderBookletSection {

    let ret = new TenderBookletSection();
    ret.Id =  input.Id != null ? input.Id : 0; 
    ret.SectionBody = input.SectionBody != null ? input.SectionBody : '';
    ret.SectionNumber = input.SectionNumber != null ? input.SectionNumber : ''; 
    ret.TenderId = input.TenderId != null ? input.TenderId : 0; 
    ret.TenderSectionId = input.TenderSectionId != null ? input.TenderSectionId : null; 
    return ret;
  }
  
  public GetData() {
    // this.sectionToDisplay;
    // let url: string = "odata/TenderTemplatesBookletSections";// + this.tenderId)
    // this.httpGeneralService.GetData(url, "(2475)", null, null)
    //   .subscribe(
    //     (data) => { 
    //       //this._filename = data; 
    //       console.log(data); },
    //   (error) => { console.log(error);}
    // );

    // "http://localhost:53166/odata/TenderBookletSections?$orderby=TenderSectionId" ("http://localhost:53166/odata/TenderBookletSections(800)")
    this.httpGeneralService.GetData("TenderTemplatesBookletSections/GetTenderTemplatesBookletSection/", "?key=" + this.tenderId, null, null).subscribe(result => {
        this.sectionToDisplay = <TenderTemplatesBookletSection>result[0];
        console.log(this.sectionToDisplay.SectionBody);
        this.DisplayExportButton = true;
      },   (error: Response) => { console.log(error)});

  }
 
  public SendData(): void {
    this._ret = "";
    //this._http.post(this._baseUrl + "api/Save/Book", this.sectionToDisplay).subscribe(r => { this._ret = <string>r; console.log(r) }, e => console.error(e))
    this.httpGeneralService.PostData("api/Save/Book", null, this.sectionToDisplay,null).subscribe(r => { this._ret = <string>r; console.log(r) }, e => console.error(e))

    this.sectionToDisplay.Id = parseInt(this.tenderId);
    this.httpGeneralService.PostData("TenderBookletSections", null, this.Convert(this.sectionToDisplay),  null).subscribe((data: any) => {
      alert("חוברת המכרז נשמרה בהצלחה <br>" + data.SectionBody);
    }, e => {
      console.log(e);
      alert(e);
    })
  }







  // public SaveData() {

  //   this._http.get<WeatherForecast[]>(this._baseUrl + 'api/Save/Get').subscribe(result => {
  //     this.forecasts = result;
  //   }, error => console.error(error));
  // }

  // public OldGetFn() {
  //   this._http.get<WeatherForecast[]>(this._baseUrl + 'api/Save/Get').subscribe(result => {
  //     this.forecasts = result;
  //   }, error => console.error(error));
  // }

  // public TestAutomaticOL(): void {
  //   let sText: string;

  //   sText = "X";
  //   // this.sectionToDisplay = new TenderSection(0, 0, 0, sText, 1);
  // }

  /*
   *
   * 
   sText =
        ` <ol> <li>Lorem ipsum.</li>
      <li>Excepteur sint occaecat cupidatat non proident:
          <ol>
              <li>sunt in culpa qui officia,</li>
              <li>deserunt mollit anim id est laborum.</li>
          </ol>
      </li></ol>`;
     
   
  */

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
