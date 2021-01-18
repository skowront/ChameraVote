class RSA:
    def __init__(self):
        self.P = 17
        self.Q = 23
        self.n = self.P * self.Q
        self.fi = (self.P-1) * (self.Q-1)
        self.e = 5
        self.d = int((2*self.fi +1 )/self.e)
        self.PublicKey = [self.e,self.n]
        self.PrivateKey = [self.d,self.n]

    def Sign(self,mPrime):
        sPrime = (mPrime ** self.d)%self.n
        return sPrime

    def Verify(self,signature,message):
        if signature**self.e == message % self.n:
            return True
        else:
            return False