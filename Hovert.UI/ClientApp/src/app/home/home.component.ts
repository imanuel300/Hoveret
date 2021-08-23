import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { DataStateChangeEvent, GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { State, process } from '@progress/kendo-data-query';
import { SpinnerVisibilityService } from 'ng-http-loader';
import { HttpGeneralService } from '../services/httpGeneralService.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public TemplatesHomegridView: GridDataResult;
  public Templates: any;
  public gridData: any[];
  public pageSize: number = 15;
  public skip: number = 0;
  public items: any[];
  public NameNewTemplate: string;
  public masMyTender: number;
  public state: State = {
    skip: this.skip,
    take: this.pageSize,
  };

  constructor(private router: Router, public HttpGeneralService: HttpGeneralService, private spinner: SpinnerVisibilityService) {
    this.HttpGeneralService.GetData('WordEditor/GetTemplatesHome', null, null, null).subscribe((data: any) => {
      console.log(data);
      this.gridData = data;
      this.loadItems();
    });
  }


  public dataStateChange(state: DataStateChangeEvent): void {
    this.state = state;
    this.TemplatesHomegridView = process(this.gridData, this.state);
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadItems();

  }

  private loadItems(): void {
    this.TemplatesHomegridView = {
      data: this.gridData.slice(this.skip, this.skip + this.pageSize),
      total: this.gridData.length
    };
  }
  onClickEdit(TemplateId: string) {
    // if (this.generalService.isMenahel) this.generalService.EditMode= true;
    this.router.navigateByUrl("/Weditor/" + TemplateId);
  }
  SaveToFile(TemplateId: string) {
    // if (this.generalService.isMenahel) this.generalService.EditMode= true;
    this.router.navigateByUrl("/Saver/" + TemplateId);
  }
  addNewTemplate() {
    this.spinner.show();
    this.HttpGeneralService.GetData("WordEditor/NewTemplate", "/?NameNewTemplate=" + this.NameNewTemplate, null, null).subscribe((data: any) => {
      console.log("data: ");
      console.log(data);
      this.gridData = this.gridData.concat(data)
      this.loadItems();
      this.spinner.hide();
      this.router.navigateByUrl("/Weditor/" + data.Id + '');
    })

  }
  duplicateRows(TenderID: number) {
    // this.dialogService.alertConfirm("המכרז ישוכפל. האם את/ה בטוח/ה?");
    // this.subscription = this.dialogService.getConfirmDialogResult().subscribe((isConfirm: boolean) => {
    //     if (!isConfirm) {
    //         console.log("no");
    //     }
    //     else {
    //         console.log("yes");
    //         let paramsArr: HashTable<any> = {};
    //         paramsArr["TenderID"] = TenderID;
    //         this.generalService.GetData(`Data`, 'duplicateRows', paramsArr, null).subscribe((data: any) => {
    //             console.log(data);
    //             this.thumbtackEvent.emit();//this.getAllSearchTenders(); + MyTendersGridComponent.getAllMyModularTenders();
    //             console.log("send event to parent");
    //         });
    //     }
    //     if (this.subscription != null) this.subscription.unsubscribe();
    // });
  }

  deleteRow(ID: number) {
    this.spinner.show();
    console.log("deleteRow: " + ID);
    this.HttpGeneralService.PostData("WordEditor/deleteRow/" + ID, null, null, null).subscribe((data: any) => {
      console.log("data: ");
      console.log(data);
      if (data == true) {
        var index = this.gridData.map(x => { return x.Id; }).indexOf(ID);
        console.log("true: "+index);
        this.gridData.splice(index, 1);
        this.loadItems();
      }
      this.spinner.hide();
    })
    // this.dialogService.alertConfirm("לאחר מחיקת המכרז לא ניתן יהיה לשחזר אותו. האם את/ה בטוח/ה?");
    // this.subscription = this.dialogService.getConfirmDialogResult().subscribe((isConfirm: boolean) => {
    //     if (!isConfirm) {
    //         console.log("no");
    //     }
    //     else {
    //         console.log("yes");
 
    //     }
    //     if (this.subscription != null) this.subscription.unsubscribe();
    // });
  }

}
