import { Pipe, PipeTransform } from '@angular/core';
import { UriConstant } from '../constants/uri-constant';

@Pipe({
  name: 'imageUri'
})
export class ImageUriPipe implements PipeTransform {

  transform(value: string, args?: any): any {
    if (value) {
      return UriConstant.ServerUri + value;
    }
    return '';
  }

}
