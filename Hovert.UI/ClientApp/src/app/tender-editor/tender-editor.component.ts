import { AfterViewInit, Component, ElementRef, HostListener, Inject, Injectable, OnInit, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EditorChangeContent, EditorChangeSelection } from 'ngx-quill'
import { HttpGeneralService } from '../services/httpGeneralService.service';
import { FormControl, FormGroup, NgForm } from '@angular/forms';
//import Quill from 'quill';
//import { CKEditor4 } from 'ckeditor4-angular/ckeditor';
import { EditorModule } from '@tinymce/tinymce-angular';
import { AlertService } from '../services/_alert'
import { ActivatedRoute } from '@angular/router';
import { TenderTemplatesBookletSection } from '../model/TenderTemplatesBookletSections';
import { SpinnerVisibilityService } from 'ng-http-loader';
import { Alert, AlertType } from './../services/_alert/alert.model';


@Component({
  selector: 'app-tender-editor',
  templateUrl: './tender-editor.component.html',
  styleUrls: ['./tender-editor.component.css']
})
//@Injectable()
export class TenderEditorComponent {
  //@ViewChild('myEditor') myEditor: any;
  htmlEditor: any;
  Id: string = null;
  isParamsId: boolean = true;
  public loader: number = 5;
  public loaderRequestResult: any = [];
  public requestResult: any = [];


  @HostListener("window:scroll", [])
  onScroll(): void {
    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight) {
      this.loaderRequestResult = this.requestResult.slice(0, this.loader = this.loader + 25); //console.log("window:scroll");
    }
  }

  constructor(route: ActivatedRoute, alertService: AlertService, private elem: ElementRef, private httpGeneralService: HttpGeneralService, private http: HttpClient, @Inject('BASE_URL') baseUrl: string, private spinner: SpinnerVisibilityService) {
    var options = { autoClose: 0, keepAfterRouteChange: false };
    let url: string = "TenderTemplateEditor/GetTenderTemplateEditor";
    route.params.subscribe(params => {
      if (params["Id"] != undefined) this.isParamsId = false;
      params["Id"] != undefined ? this.Id = "/?key=" + params["Id"] + "" : this.Id = null; console.log(params);
    });
    this.httpGeneralService.GetData(url, this.Id, null, null).subscribe((data: any) => {

      //data.value == null ? this.requestResult[0] = data : this.requestResult = data.value;
      this.requestResult = data
      for (let i = 0; i < this.requestResult.length; i++) {
        this.requestResult[i].IsHtml = false;
        this.requestResult[i].IsConditions = false;
      }
      console.log("data: " + url);
      console.log(this.requestResult);
      this.loaderRequestResult = this.requestResult.slice(0, this.loader)
      //SectionBody + Id
    }, error => {
      alertService.error('שגיאה בקבלת נתונים מהשרת', options);
    })
  }

  onSubmitSave(myTenderTemplatesBookletSection: TenderTemplatesBookletSection) {
    this.spinner.show();
    let url: string = "TenderTemplateEditor/SaveTenderTemplateEditor";
    console.log(myTenderTemplatesBookletSection);
    let tenderTemplatesBookletSection = new TenderTemplatesBookletSection(myTenderTemplatesBookletSection);
    this.httpGeneralService.PostData(url, null, tenderTemplatesBookletSection, null).subscribe((data: any) => {
      console.log("data: ");
      console.log(data);
      this.loaderRequestResult.SectionBody = data.SectionBody;
      this.spinner.hide();
    })
  }

  public editorForm: FormGroup;
  public Interval: any;
  ngOnInit() {
    //   var icons = Quill.import("ui/icons");
    //   icons["undo"] = `<svg viewbox="0 0 18 18">
    //   <polygon class="ql-fill ql-stroke" points="12 10 14 12 16 10 12 10"></polygon>
    //   <path class="ql-stroke" d="M9.91,13.91A4.6,4.6,0,0,1,9,14a5,5,0,1,1,5-5"></path>
    // </svg>`;
    //   icons["redo"] = `<svg viewbox="0 0 18 18">
    //   <polygon class="ql-fill ql-stroke" points="6 10 4 12 2 10 6 10"></polygon>
    //   <path class="ql-stroke" d="M8.09,13.91A4.6,4.6,0,0,0,9,14,5,5,0,1,0,4,9"></path>
    // </svg>`;


    let that = this;
    if (that.Interval == undefined) {
      that.Interval = setInterval(function () {
        // document= elem.nativeElement
        if (that.elem.nativeElement.querySelectorAll('.ql-toolbar').length > 0) {
          var placeholderPickerItems = Array.prototype.slice.call(that.elem.nativeElement.querySelectorAll('.ql-placeholder .ql-picker-item'));
          placeholderPickerItems.forEach(item => item.textContent = item.dataset.value);
          that.elem.nativeElement.querySelector('.ql-placeholder .ql-picker-label').innerHTML = 'בוקמארק' + that.elem.nativeElement.querySelector('.ql-placeholder .ql-picker-label').innerHTML;


          clearInterval(that.Interval);
          that.Interval = undefined;
        }
      }, 500);
    }



    this.editorForm = new FormGroup({
      'editor': new FormControl(null)
    })
    //this.editorForm.get('editor').setValue('<h3>כתוב משהוא..</h3>');
  }

  ngAfterViewChecked() {
  }
  // ngAfterViewInit() {
  //   let Editor = this.myEditor;
  //   console.log("ngAfterViewInit: Editor " + Editor);
  // }
  ngOnDestroy() {
    //console.log("I'm destroying details!!");
  }
  ngAfterContentInit() {
    //console.log("View content is triggered");
  }

  insert = (arr, index, newItem) => [
    ...arr.slice(0, index),// part of the array before the specified index
    newItem,// inserted item
    ...arr.slice(index) // part of the array after the specified index
  ];
  AddNewRow(TenderSectionId: number, i: number) {
    console.log("AddNewRow(" + TenderSectionId + ")");
    this.spinner.show();
    let url: string = "TenderTemplateEditor/AddNewRow/?id=" + TenderSectionId;
    this.httpGeneralService.PostData(url, null, null, null).subscribe((data: any) => {
      console.log("data: ");
      console.log(data);
      this.spinner.hide();
      let tenderTemplatesBookletSection = new TenderTemplatesBookletSection(null);
      tenderTemplatesBookletSection.Id = data;
      tenderTemplatesBookletSection.TenderSectionId = TenderSectionId + 1;
      tenderTemplatesBookletSection.TenderId = 0;
      this.loaderRequestResult = this.insert(this.loaderRequestResult, i + 1, tenderTemplatesBookletSection);
    })
  }

  delete = (arr, index) => [
    ...arr.slice(0, index),// part of the array before the specified index
    ...arr.slice(index + 1) // part of the array after the specified index
  ];
  DellRow(id: number, i: number) {
    console.log("DellRow(" + id + ")");
    if (window.confirm("האם להמשיך במחיקה?")) {
      this.spinner.show();
      let url: string = "TenderTemplateEditor/Delete/?id=" + id;
      this.httpGeneralService.PostData(url, null, null, null).subscribe((data: any) => {
        console.log("data: ");
        console.log(data);
        this.spinner.hide();
        this.loaderRequestResult = this.delete(this.loaderRequestResult, i);//this.loaderRequestResult.slice(i, 1);
      })
    }
  }

  selectItem(item: any) {
    console.log(item);
    item.srcElement.style.height = 'auto';
    item.srcElement.style.height = item.srcElement.scrollHeight + 'px';
  }

 

  // public editorOptions = {
  //   toolbar: {
  //     container: [
  //       [{ redo: 'redo' }, { undo: 'undo' }],
  //       ['bold', 'italic', 'underline'],        //, 'strike', 'formula' toggled buttons
  //       // ['blockquote', 'code-block'],
  //       [{ 'align': [] }],
  //       //[{ 'header': 1 }, { 'header': 2 }],               // custom button values
  //       [{ 'list': 'ordered' }, { 'list': 'bullet' }],
  //       [{ 'script': 'sub' }, { 'script': 'super' }],      // superscript/subscript
  //       [{ 'indent': '-1' }, { 'indent': '+1' }],          // outdent/indent
  //       [{ 'direction': 'rtl' }],                         // text direction
  //       [{ 'size': ['small', false, 'large', 'huge'] }],  // custom dropdown
  //       [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
  //       [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
  //       [{ 'font': [] }],
  //       //['showHtml'], ['customControl'],
  //       ['link'],                        // link and image, video
  //       [{ 'placeholder': ['GuestName', 'HotelName'] }]
  //     ],
  //     handlers: {
  //       "customControl": function () {
  //         console.log('customControl was clicked');
  //         const cursorPosition = this.quill.getSelection().index;
  //         this.quill.insertText(cursorPosition, 'value');
  //         this.quill.setSelection(cursorPosition);

  //       }, //this.onCustomControlClick,
  //       "placeholder": function (value) {
  //         console.log('placeholder was clicked');
  //         if (value) {
  //           const cursorPosition = this.quill.getSelection().index;
  //           this.quill.insertText(cursorPosition, value);
  //           this.quill.setSelection(cursorPosition + value.length);
  //         }
  //       },
  //       redo() { this.quill.history.redo(); },
  //       undo() { this.quill.history.undo(); }
  //     },
  //     history: { delay: 2000, maxStack: 500, userOnly: true }

  //   }
  // };

  // getSelectionHtml() {
  //   let quill = new Quill('#editor');
  //   var selection = quill.getSelection();
  //   var selectedContent = quill.getContents(selection.index, selection.length);
  //   var tempContainer = document.createElement('div')
  //   var tempQuill = new Quill(tempContainer);
  //   tempQuill.setContents(selectedContent);
  //   console.log(tempContainer.querySelector('.ql-editor').innerHTML);
  // }

  // onEditorReady() {
  //   console.log('editor ready!')
  //   document.addEventListener("DOMContentLoaded", function (event) {
  //     var placeholderPickerItems = Array.prototype.slice.call(document.querySelectorAll('.ql-placeholder .ql-picker-item'));
  //     placeholderPickerItems.forEach(item => item.textContent = item.dataset.value);
  //     document.querySelector('.ql-placeholder .ql-picker-label').innerHTML = 'Insert placeholder' + document.querySelector('.ql-placeholder .ql-picker-label').innerHTML;
  //   });
  // }

  // onContentChanged({ quill, html, text }) {
  //   console.log('quill content is changed!', quill, html, text);
  //   if (html) this.htmlEditor = html;
  // }


}


