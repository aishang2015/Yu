import { Pipe, PipeTransform } from '@angular/core';
import { UriConstant } from '../constants/uri-constant';

@Pipe({
  name: 'avatarUri'
})
export class AvatarImagePipe implements PipeTransform {

  transform(value: string, args?: any): any {
    if (value) {
      return UriConstant.AvatarBaseUri + value;
    }
    return '';
  }

}
