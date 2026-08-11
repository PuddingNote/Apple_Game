using System;

// "1.1.1" vs "0.9.9" 같은 버전 문자열을 안전하게 비교하는 순수 유틸 함수
// 강제 업데이트(VersionGate)는 실패 시 무조건 통과(fail-open)해야 하므로,
// 파싱 실패를 예외 대신 TryParse의 반환값으로 알려서 호출부가 판단하게 한다.
public static class AppVersion
{

    // "1.1.1" -> (1, 1, 1) 파싱. 실패하면 false를 반환한다 (예외를 던지지 않음)
    public static bool TryParse(string versionString, out (int major, int minor, int patch) version)
    {
        version = (0, 0, 0);

        if (string.IsNullOrWhiteSpace(versionString))
        {
            return false;
        }

        string[] parts = versionString.Trim().Split('.');
        if (parts.Length == 0)
        {
            return false;
        }

        int[] numbers = new int[3];
        for (int i = 0; i < 3; i++)
        {
            if (i >= parts.Length)
            {
                numbers[i] = 0;
                continue;
            }

            if (!int.TryParse(parts[i], out numbers[i]) || numbers[i] < 0)
            {
                return false;
            }
        }

        version = (numbers[0], numbers[1], numbers[2]);
        return true;
    }

    // current가 minVersion보다 낮으면(업데이트 필요) true.
    // 둘 중 하나라도 파싱에 실패하면 판단할 수 없으므로 false(업데이트 불필요 취급 = fail-open)를 반환한다.
    public static bool IsLowerThan(string current, string minVersion)
    {
        if (!TryParse(current, out var currentVersion) || !TryParse(minVersion, out var minVersionParsed))
        {
            return false;
        }

        return Compare(currentVersion, minVersionParsed) < 0;
    }

    private static int Compare((int major, int minor, int patch) a, (int major, int minor, int patch) b)
    {
        int majorCompare = a.major.CompareTo(b.major);
        if (majorCompare != 0) return majorCompare;

        int minorCompare = a.minor.CompareTo(b.minor);
        if (minorCompare != 0) return minorCompare;

        return a.patch.CompareTo(b.patch);
    }

}
