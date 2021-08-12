import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { GuideComponent } from './guide/guide.component';
import { SaverComponent } from './saver/saver.component';
import { TenderTemplateComponent } from './tender-template/tender-template.component';
import { TenderEditorComponent } from './tender-editor/tender-editor.component';
import { WordEditorComponent } from './word-editor/word-editor.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component'

const routes: Routes = [
  { path: 'editor', component: TenderEditorComponent },
  { path: 'editor/:Id', component: TenderEditorComponent },
 // { path: 'Weditor', component: WordEditorComponent },
  { path: 'Weditor/:Id', component: WordEditorComponent },
  { path: 'template', component: TenderTemplateComponent },
  // { path: 'test', component: TestComponent },
  //{ path: 'Saver', component: SaverComponent },
  { path: 'Saver/:Id', component: SaverComponent },
  { path: '', redirectTo: 'Home', pathMatch: 'full' },
 // { path: '/', redirectTo: '/Home' },
  { path: 'Home', component: HomeComponent },
  { path: 'Guide', component: GuideComponent },
  { path: '404', component: PageNotFoundComponent }
  ,{ path: '**', redirectTo: 'Home' }
 // ,{ path: '', redirectTo: '/login', pathMatch: 'full'}
]; 

export const AppRoute = RouterModule.forRoot(routes); 
/*, canDeactivate: [CanDeactivateGuard]*/

//ng build --configuration=production && copy dist \\svdweb12\Hoveret /y
//ng serve --source-map --optimization=false

//SELECT * INTO [dbo].TenderTemplatesBookletSections FROM [dbo].TenderTemplatesBookletSectionsAspose
