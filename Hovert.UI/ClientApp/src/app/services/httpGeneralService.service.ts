import { Injectable } from "@angular/core";
import { HttpClientModule, HttpHeaders } from '@angular/common/http';
import { HttpClient, HttpEvent, HttpHandler, HttpParams, HttpRequest, HttpResponse, HttpInterceptor, HttpErrorResponse } from "@angular/common/http";
import { environment } from "../../environments/environment";
import { Observable, of, Subject, throwError } from "rxjs";
import { catchError, map, retry, tap } from 'rxjs/operators';

@Injectable()
export class HttpGeneralService {
  [x: string]: any;
  baseURL = this.appWebServiceUrl = `http://${environment.SERVER_NAME}:${environment.SITE_PORT}/api`;

  constructor(private http: HttpClient) { }

  handleError(error) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      // Get client-side error
      errorMessage = error.error.message;
    } else {
      // Get server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    // window.alert(errorMessage);
    return throwError(errorMessage);
  }

  PostData<T>(controller: string, action: string, bodyParams: any, uriParamsArr: HttpHeaders = null) {
    let url;
    if (action != null)
      url = this.baseURL + `/` + controller + action;
    else
      url = this.baseURL + `/` + controller;
    if (uriParamsArr == null) uriParamsArr = new HttpHeaders({ 'Content-Type': 'application/json', 'Accept': 'application/json' });
    return this.http.post<T>(url, bodyParams, { headers: uriParamsArr }).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  PutData<T>(controller: string, action: string, bodyParams: any, uriParamsArr: HttpParams = null) {
    let url;
    if (action != null)
      url = this.baseURL + `/` + controller + action;
    else
      url = this.baseURL + `/` + controller;
    return this.http.put<T>(url, bodyParams, { params: uriParamsArr }).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  GetData<T>(controller: string, action: string, uriParamsArr: HttpParams, id: any) {
    let url;
    if (action != null)
      url = this.baseURL + `/` + controller + action;
    else
      url = this.baseURL + `/` + controller;
    if (id != null) url = url + `/` + id;
    return this.http.get<T>(url, { params: uriParamsArr }).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }




  public loadData(): Observable<ITenderSection> {
    console.log("In Service");
    return this.http.get<ITenderSection>("http://localhost:53166/odata/TenderSections(800)");
  }

}

export interface ITenderSection {
  Id: number;
  Section: string;
  ParentSection: string;
  Text: string;
  ConditionId: boolean;
}

export class TenderSection implements ITenderSection {
  constructor(Id, Section, ParentSection, Text, ConditionId) {
    this.Id = Id;
    this.Section = Section;
    this.ParentSection = ParentSection;
    this.Text = Text;
    this.ConditionId = ConditionId;
  }
  Id: number;
  Section: string;
  ParentSection: string;
  Text: string;
  ConditionId: boolean;

}
