import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppRoute } from './app.routing';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { SafePipe } from './pipes/sanitizer-pipe';
import { SafeurlPipe } from './pipes/safeurl';
import { HttpGeneralService } from './services/httpGeneralService.service';
import { TenderTemplateComponent } from './tender-template/tender-template.component';
import { TenderEditorComponent } from './tender-editor/tender-editor.component';
import { TenderBookletSection } from './model/TenderBookletSection';
//import { QuillModule } from 'ngx-quill';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { NgHttpLoaderModule } from 'ng-http-loader';
import { AlertModule } from './services/_alert';
import { CKEditorModule } from 'ckeditor4-angular';
import { EditorModule, TINYMCE_SCRIPT_SRC } from '@tinymce/tinymce-angular';

@NgModule({
  imports: [
    //CKEditorModule,
    EditorModule,
    HttpClientModule, // <============ (Perform HTTP requests with this module)
    NgHttpLoaderModule.forRoot(), // <============ Don't forget to call 'forRoot()'!
   // QuillModule.forRoot(),
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    AlertModule,
    AppRoute,
  ],
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    SafePipe,
    SafeurlPipe,
    TenderTemplateComponent,
    TenderEditorComponent,
    PageNotFoundComponent
  ],
  providers: [
    TenderBookletSection,
    HttpGeneralService,
    { provide: TINYMCE_SCRIPT_SRC, useValue: 'tinymce/tinymce.min.js' }
    //,{ provide: HTTP_INTERCEPTORS, useClass: HttpConfigInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
//