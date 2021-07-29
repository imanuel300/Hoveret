import { Pipe, PipeTransform } from '@angular/core';
//import { DomSanitizer } from '@angular/platform-browser';

@Pipe({
  name: 'translate',
  pure: false,
})
export class TranslatePipe implements PipeTransform {
  constructor() {}

  transform(value: any, args?: any): any {
var array  = {
  //"MULTILEVEL": "רמת מספור פסקה",
  //"TenderSectionId": "סדר הופעה",
  //"SectionNumber": "מספור פסקה",
  //"PARAGRAPH": "זו פסקה",
  "CM_2": "דיור מוגן",
  "CM_3": "בנה ביתך - קרקע",
  "CM_4": "בנה ביתך - הגרלה",
  "CM_5": "הרשמה והגרלה",
  "CM_6": "הרשמה והגרלה",
  "CM_7": "ר.מ.י קרקע",
  "CM_8": "בנה דירתך",
  "CM_9": "מחיר למשתכן - ישן",
  "CM_10": "מימון מלא",
  "CM_11": "בניה להשכרה",
  "CM_12": "ועדת ברודט",
  "CM_13": "אגודה שיתופית",
  "CM_14": "יקבע בהמשך",
  "CM_15": "ועדת קבלה",
  "CM_16": "מחיר מטרה",
  "CM_17": "מחיר מטרה - ר.מ.י",
  "CM_18": "מחיר למשתכן",
  "CM_19": "מחיר למשתכן - ר.מ.י",
  "CM_20": "מכרז מסחר ו/או תעסוקה",
  "CD_1": "חברה מפתחת",
  "CD_2": "בניה ופתוח",
  "CD_3": "רשות מקומית",
  "CD_4": "משרד הבינוי והשיכון",
  "CD_5": "פתוח בוצע",
  "CD_6": "בניה והשלמת פתוח",
  "CD_7": "בניה ופתוח לראש השטח",
  "CD_8": "חברה מנהלת",
  "onlyDevPayBigZeroo": "פיתוח >0 ישן וציבור =0",
  "HeadLand": "ראש שטח",
  "PublicBuildingBihZero": "מתחם משלם מוסדות.צ",
  "OldByNewBigZero": "מתחם משלם ישן מול חדש",
  "Ayosh": 'איו"ש',
  "Sipsud40K":"סיבסוד 40K",
  "ProjectMoreOne":"מעל יחידת דיור אחת",
  "ProjectMore4":"מעל 4 יחידות דיור",
  "NOConditions":"ללא תנאים"
  

}
    return array[value];
  }
}