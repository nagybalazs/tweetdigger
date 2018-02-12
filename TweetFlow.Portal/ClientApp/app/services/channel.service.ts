import { Injectable } from '@angular/core';
import { Channel } from '../classes/classes';

@Injectable()
export class ChannelService {

    public static getChannels(): Channel[] {
        return [
            { endpoint: 'bitcoin', name: '#bitcoin' },
            { endpoint: 'ethereum', name: '#ethereum' }
        ];
    }

}