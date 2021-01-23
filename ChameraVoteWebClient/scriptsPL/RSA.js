class RSA
{
    static n = 35;
    static PublicKey = [7,RSA.n];
    static ModInverse(a, m) {
        // validate inputs
        [a, m] = [Number(a), Number(m)]
        if (Number.isNaN(a) || Number.isNaN(m)) {
          return NaN // invalid input
        }
        a = (a % m + m) % m
        if (!a || m < 2) {
          return NaN // invalid input
        }
        // find the gcd
        const s = []
        let b = m
        while(b) {
          [a, b] = [b, a % b]
          s.push({a, b})
        }
        if (a !== 1) {
          return NaN // inverse does not exists
        }
        // find the inverse
        let x = 1
        let y = 0
        for(let i = s.length - 2; i >= 0; --i) {
          [x, y] = [y,  x - y * Math.floor(s[i].a / s[i].b)]
        }
        return (y % m + m) % m
      }
}

