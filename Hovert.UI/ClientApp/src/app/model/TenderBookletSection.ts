import { Injectable } from "@angular/core";

  

interface ITenderBookletSection
 {

  Id: number,
  TenderSectionId: number,
  TenderId: number,
  SectionNumber: string,
  SectionBody: string
  versionBooklat : number;
}

@Injectable()
export class TenderBookletSection implements ITenderBookletSection {
  constructor() {
    this.Id = 0;
    this.TenderSectionId = 0;
    this.TenderId = 0;
    this.SectionNumber = "";
    this.SectionBody = "";
    this.versionBooklat = null;
  }
  Id: number;
  TenderSectionId: number;
  TenderId: number;
  SectionNumber: string;
  SectionBody: string;
  versionBooklat:number;
}
