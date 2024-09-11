import { Pipe, PipeTransform } from '@angular/core';
import { format } from 'date-fns';

@Pipe({
  name: 'time24'
})
export class Time24Pipe implements PipeTransform {

  transform(value: Date | string | number): string {
    return format(new Date(value), 'HH:mm');

  }

}
