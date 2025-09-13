import type { NLocale } from '../locales/common/enUS';
import type { NDateLocale } from '../locales/date/enUS';
import { type Ref } from 'vue';
export default function useLocale<T extends keyof NLocale>(ns: T): {
    localeRef: Ref<NLocale[T]>;
    dateLocaleRef: Ref<NDateLocale>;
};
