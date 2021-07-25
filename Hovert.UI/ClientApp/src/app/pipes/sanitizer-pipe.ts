import { Pipe, PipeTransform} from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";


@Pipe({ name: 'safeHtml' })
export class SafePipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) { }

  transform(style:any):any {
    if (style != null) {
      return this.sanitizer.bypassSecurityTrustHtml(style);
    }
    else return "";
  }
}
